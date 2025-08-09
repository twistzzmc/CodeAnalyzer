using System.Collections.Concurrent;
using System.Diagnostics;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Models.Stats.Data;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Dtos;
using CodeAnalyzer.Parser.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser;

public class CodeParser(IWarningRegistry warningRegistry, ILogger logger)
{
    private readonly ConcurrentBag<Dictionary<IdentifierDto, CboDto>> _cboMaps = [];
    private readonly ConcurrentBag<ClassModelsBuilder> _classBuilders = [];
    private CSharpCompilation? _compilation;
    private IProgress? _progress;
    private CancellationTokenSource? _cancellationTokenSource;

    public static async Task<IEnumerable<ClassModel>> ParseAsync(
        IWarningRegistry warningRegistry,
        ILogger logger,
        IEnumerable<FileDto> codes)
    {
        return await new CodeParser(warningRegistry, logger).WalkAsync(codes);
    }

    private async Task<IEnumerable<ClassModel>> WalkAsync(IEnumerable<FileDto> codes)
    {
        _compilation = Compile(codes);
        _progress = logger.OpenProgress(_compilation.SyntaxTrees.Length, "Asynchroniczne Przeszukiwanie drzew");

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = 6,
                CancellationToken = cancellationToken
            };
            
            Stopwatch stopwatch = Stopwatch.StartNew();

            await Parallel.ForEachAsync(_compilation.SyntaxTrees, options, VisitRootAsync);

            Dictionary<IdentifierDto, CboDto> cboMap = JoinCboMaps();
            ClassModelsBuilder builder = JoinClassBuilders();
            List<ClassModel> models = builder.Build(logger).ToList();
            foreach (ClassModel model in models)
            {
                model.Stats.Cbo = cboMap.TryGetValue(model.Identifier, out CboDto? cbo) ? cbo : CboDto.Empty;
            }
    
            stopwatch.Stop();
            logger.Success($"Pomyślnie przeszukano drzewa. Czas trwania: {stopwatch.ElapsedMilliseconds / 1000} s");

            return models;
        }
        finally
        {
            logger.CloseLevel();
        }
    }
    
    private async ValueTask VisitRootAsync(SyntaxTree tree, CancellationToken cancellationToken = default)
    {
        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            ArgumentNullException.ThrowIfNull(_compilation, nameof(_compilation));
            ArgumentNullException.ThrowIfNull(_progress, nameof(_progress));

            CodeWalker walker = new(new CollectorFactory(warningRegistry), _compilation);
            SyntaxNode rootNode = await tree.GetRootAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            walker.Visit(rootNode);
            _cboMaps.Add(walker.CboMap);
            _classBuilders.Add(walker.ClassBuilder);
    
            stopwatch.Stop();
            logger.Info(_progress, $"Chodzenie po drzewie {tree.FilePath} [{tree.Length}] [{stopwatch.ElapsedMilliseconds} ms]");
        }
        catch (Exception ex)
        {
            logger.Exception(ex);
            
            if (!cancellationToken.IsCancellationRequested && _cancellationTokenSource is not null)
            {
                await _cancellationTokenSource.CancelAsync();
            }
            
            throw;
        }
    }

    private Dictionary<IdentifierDto, CboDto> JoinCboMaps()
    {
        Dictionary<IdentifierDto, CboDto> joinedMap = [];

        foreach (Dictionary<IdentifierDto, CboDto> map in _cboMaps)
        {
            foreach (KeyValuePair<IdentifierDto, CboDto> pair in map)
            {
                if (joinedMap.TryGetValue(pair.Key, out CboDto? joined))
                {
                    joinedMap[pair.Key] = JoinCbo(pair.Value, joined);
                    continue;
                }
                
                joinedMap[pair.Key] = pair.Value;
            }
        }
        
        return joinedMap;
    }

    private ClassModelsBuilder JoinClassBuilders()
    {
        ClassModelsBuilder joinedClassBuilders = new();

        foreach (ClassModelsBuilder builder in _classBuilders)
        {
            builder.Fill(joinedClassBuilders);
        }
        
        return joinedClassBuilders;
    }

    private static CboDto JoinCbo(CboDto cbo1, CboDto cbo2)
    {
        return new CboDto(cbo1.Cbo + cbo2.Cbo, [..cbo1.ReferencesTypes, ..cbo2.ReferencesTypes]);
    }

    private static CSharpCompilation Compile(IEnumerable<FileDto> codes)
    {
        List<SyntaxTree> syntaxTrees = codes.Select(
            code => CSharpSyntaxTree.ParseText(code.Data, path: code.Path)).ToList();

        IEnumerable<PortableExecutableReference> references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));
        
        return CSharpCompilation.Create("FullAnalysis")
            .AddSyntaxTrees(syntaxTrees)
            .AddReferences(references);
    }
}