using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void AnalyzeTooLongMethods_OnClick(object? sender, RoutedEventArgs e)
    {
        AnalyzeTooLongMethods.IsEnabled = false;

        try
        {
            List<ClassModel> models = Parse().ToList();

            List<MethodResultDto> mtlResults = [];
            List<GodObjectResultDto> godObjectResults = [];

            MtlAnalyzer mtlAnalyzer = new();
            GodObjectAnalyzer godObjectAnalyzer = new(models);

            foreach (ClassModel model in models)
            {
                ResultLogger.AddEntry(new ClassEntryBuilder().Build(model));

                mtlResults.AddRange(mtlAnalyzer.Analyze(model.Methods));
            }

            MethodTooLongLogger.Log(ResultLogger, mtlResults);
            new GodObjectLogger(ResultLogger).Log(godObjectResults);
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

    private IEnumerable<ClassModel> Parse()
    {
        if (TryParseFile(out IEnumerable<ClassModel> fileModels))
        {
            return fileModels;
        }

        return TryParseFolder(out IEnumerable<ClassModel> folderModels)
            ? folderModels
            : [];
    } 

    private bool TryParseFile(out IEnumerable<ClassModel> models)
    {
        string filePath = CodePathProvider.ProvideFile();

        if (string.IsNullOrEmpty(filePath))
        {
            models = [];
            return false;
        }
        
        string code = File.ReadAllText(filePath);
        models = CodeParser.Parse(WarningRegistry, Logger, code);
        return true;
    }

    private bool TryParseFolder(out IEnumerable<ClassModel> models)
    {
        string[] excludedFolders = ["obj", "bin", "Tests", "Generated"];
        string folderPath = CodePathProvider.ProvideFolder();
        models = [];

        if (string.IsNullOrEmpty(folderPath))
        {
            return false;
        }
        
        string[] files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
            .Where(path => !excludedFolders.Any(folder =>
                    path.Split(Path.DirectorySeparatorChar)
                        .Contains(folder)))
            .ToArray();

        IEnumerable<string> codes = files.Select(File.ReadAllText);
        models = CodeParser.Parse(WarningRegistry, Logger, codes);
        return true;
    }
}