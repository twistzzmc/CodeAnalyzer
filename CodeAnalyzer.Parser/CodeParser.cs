using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser;

public class CodeParser(IWarningRegistry warningRegistry, ILogger logger)
{
    public IEnumerable<ClassModel> Parse(IEnumerable<string> codes)
    {
        return Walk(Compile(codes));
    }

    public IEnumerable<ClassModel> Parse(string code)
    {
        return Walk(Compile([code]));
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, ILogger logger, string code)
    {
        return new CodeParser(warningRegistry, logger).Parse(code);
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, ILogger logger, IEnumerable<string> codes)
    {
        return new CodeParser(warningRegistry, logger).Parse(codes);
    }

    private IEnumerable<ClassModel> Walk(CSharpCompilation compilation)
    {
        CodeWalker walker = new(warningRegistry, compilation);

        try
        {
            IProgress progress = logger.OpenProgress(compilation.SyntaxTrees.Length, "Przeszukiwanie drzew");
            foreach (SyntaxTree tree in compilation.SyntaxTrees)
            {
                logger.Info(progress, "Chodzenie po drzewie {0}", tree.Length);
                walker.Visit(tree.GetRoot());
            }

            logger.Success("Pomyślnie udało się przejść po drzewie");
        }
        catch (Exception e)
        {
            logger.Exception(e);
            throw;
        }
        finally
        {
            logger.CloseLevel();
        }
        
        logger.Info("Budowanie znalezionych klas");
        return walker.CollectClassModels();
    }

    private CSharpCompilation Compile(IEnumerable<string> codes)
    {
        List<SyntaxTree> syntaxTrees = codes.Select(code => CSharpSyntaxTree.ParseText(code)).ToList();

        IEnumerable<PortableExecutableReference> references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));
        
        return CSharpCompilation.Create("FullAnalysis")
            .AddSyntaxTrees(syntaxTrees)
            .AddReferences(references);
    }
}