using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.Core.Analyzers;
using CodeAnalyzer.Core.Analyzers.Dtos.Result;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
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

    public static readonly StyledProperty<ILoggerUi> LoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILoggerUi>(nameof(Logger));

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

    public ILoggerUi Logger
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
        try
        {
            List<MethodResultDto> results = [];
            List<ClassModel> models = Parse().ToList();
            
            foreach (ClassModel model in models)
            {
                Logger.Log(new ClassEntryBuilder().Build(model));

                MethodAnalyzer analyzer = new();
                results.AddRange(analyzer.Analyze(model.Methods));
            }
            
            MethodTooLongLogger.Log(Logger, results);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
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
        models = CodeParser.Parse(WarningRegistry, code);
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
        models = CodeParser.Parse(WarningRegistry, codes);
        return true;
    }
}