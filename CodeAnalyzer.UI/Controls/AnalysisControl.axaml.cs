using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser;
using CodeAnalyzer.UI.AnalysisLoggers;
using CodeAnalyzer.UI.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class AnalysisControl : UserControl
{
    private sealed record ClassModelResult(bool IsSuccess, IEnumerable<ClassModel> Models);
    
    public static readonly StyledProperty<IWarningRegistry> WarningRegistryProperty =
        AvaloniaProperty.Register<AnalysisControl, IWarningRegistry>(nameof(WarningRegistry));

    public static readonly StyledProperty<ICodePathProvider> CodePathProviderProperty =
        AvaloniaProperty.Register<AnalysisControl, ICodePathProvider>(nameof(CodePathProvider));

    public static readonly StyledProperty<ILoggerUi> ResultLoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILoggerUi>(nameof(ResultLogger));
    
    public static readonly StyledProperty<ILogger> LoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILogger>(nameof(Logger));

    public IWarningRegistry WarningRegistry
    {
        get => GetValue(WarningRegistryProperty);
        set => SetValue(WarningRegistryProperty, value);
    }

    public ICodePathProvider CodePathProvider
    {
        get => GetValue(CodePathProviderProperty);
        set => SetValue(CodePathProviderProperty, value);
    }

    public ILoggerUi ResultLogger
    {
        get => GetValue(ResultLoggerProperty);
        set => SetValue(ResultLoggerProperty, value);
    }

    public ILogger Logger
    {
        get => GetValue(LoggerProperty);
        set => SetValue(LoggerProperty, value);
    }

    public AnalysisControl()
    {
        InitializeComponent();
    }

    private async void AnalyzeTooLongMethods_OnClick(object? sender, RoutedEventArgs e)
    {
        AnalyzeTooLongMethods.IsEnabled = false;

        try
        {
            await Task.Run(async () =>
            {
                List<ClassModel> models = (await Parse()).ToList();

                List<MethodResultDto> mtlResults = [];
                List<GodObjectResultDto> godObjectResults = [];

                MtlAnalyzer mtlAnalyzer = new();
                GodObjectAnalyzer godObjectAnalyzer = new(models);

                ILoggerUi resultLogger = await GetResultLogger();
                
                foreach (ClassModel model in models)
                {
                    resultLogger.AddEntry(new ClassEntryBuilder().Build(model));

                    mtlResults.AddRange(mtlAnalyzer.Analyze(model.Methods));
                    godObjectResults.Add(godObjectAnalyzer.Analyze(model));
                }
                
                MethodTooLongLogger.Log(resultLogger, mtlResults);
                new GodObjectLogger(resultLogger).Log(godObjectResults);
            });
        }
        catch (Exception ex)
        {
            ResultLogger.AddEntry(ex);
        }
        finally
        {
            AnalyzeTooLongMethods.IsEnabled = true;
        }
    }

    private async Task<IEnumerable<ClassModel>> Parse()
    {
        ClassModelResult fileModels = await TryParseFile();
        if (fileModels.IsSuccess)
        {
            return fileModels.Models;
        }

        ClassModelResult folderModels = await TryParseFolder();
        return folderModels.Models;
    } 

    private async Task<ClassModelResult> TryParseFile()
    {
        ICodePathProvider codePathProvider = await GetCodePathProvider();
        string filePath = (await GetCodePathProvider()).ProvideFile();

        if (string.IsNullOrEmpty(filePath))
        {
            return new ClassModelResult(false, []);
        }
        
        IWarningRegistry warningRegistry = await GetWarningRegistry();
        ILogger logger = await GetLogger();
        
        string code = await File.ReadAllTextAsync(filePath);
        return new ClassModelResult(true, CodeParser.Parse(warningRegistry, logger, code));
    }

    private async Task<ClassModelResult> TryParseFolder()
    {
        string[] excludedFolders = ["obj", "bin", "Tests", "Generated"];
        string folderPath = (await GetCodePathProvider()).ProvideFolder();

        if (string.IsNullOrEmpty(folderPath))
        {
            return new ClassModelResult(false, []);
        }
        
        string[] files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
            .Where(path => !excludedFolders.Any(folder =>
                    path.Split(Path.DirectorySeparatorChar)
                        .Contains(folder)))
            .ToArray();

        IWarningRegistry warningRegistry = await GetWarningRegistry();
        ILogger logger = await GetLogger();
        
        IEnumerable<string> codes = files.Select(File.ReadAllText);
        return new ClassModelResult(true, CodeParser.Parse(warningRegistry, logger, codes));
    }

    private async Task<IWarningRegistry> GetWarningRegistry()
        => await Dispatcher.UIThread.InvokeAsync(() => WarningRegistry);
    
    private async Task<ILoggerUi> GetResultLogger()
        => await Dispatcher.UIThread.InvokeAsync(() => ResultLogger);
    
    private async Task<ILogger> GetLogger()
        => await Dispatcher.UIThread.InvokeAsync(() => Logger);
    
    private async Task<ICodePathProvider> GetCodePathProvider()
        => await Dispatcher.UIThread.InvokeAsync(() => CodePathProvider);
}