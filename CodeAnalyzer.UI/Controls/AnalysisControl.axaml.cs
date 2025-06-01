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
using CodeAnalyzer.UI.Analysis;
using CodeAnalyzer.UI.Analysis.Loggers;
using CodeAnalyzer.UI.Base;
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
    
    private readonly AsyncCodeReader _codeReader = new();
    private readonly AsyncAnalyzerHelper _analyzerHelper = new();

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

    private async void AnalyzeGodObjects_OnClick(object? sender, RoutedEventArgs e)
    {
        ChangeButtonsEnable(false);

        try
        {
            await _analyzerHelper.RunGodObjectAnalysis(Logger, ResultLogger);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
        finally
        {
            ChangeButtonsEnable(true);
        }
    }

    private async void AnalyzeLoadedFiles_OnClick(object? sender, RoutedEventArgs e)
    {
        ChangeButtonsEnable(false);

        try
        {
            IEnumerable<string> codes = await _codeReader.ReadCode(CodePathProvider);
            await _analyzerHelper.ParseCode(WarningRegistry, Logger, codes);
            await _analyzerHelper.LogModels(ResultLogger);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
        finally
        {
            ChangeButtonsEnable(true);
        }
    }

    private async void AnalyzeTooLongMethods_OnClick(object? sender, RoutedEventArgs e)
    {
        ChangeButtonsEnable(false);

        try
        {
            await _analyzerHelper.RunMtlAnalysis(ResultLogger);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
        finally
        {
            ChangeButtonsEnable(true);
        }
    }

    private async Task<IEnumerable<ClassModel>> Parse()
    {
        ICodePathProvider codePathProvider = await GetCodePathProvider();
        IWarningRegistry warningRegistry = await GetWarningRegistry();
        ILogger logger = await GetLogger();
        
        IEnumerable<string> codes = await _codeReader.ReadCode(codePathProvider);
        return CodeParser.Parse(warningRegistry, logger, codes);
    }

    private void ChangeButtonsEnable(bool enable)
    {
        AnalyzeLoadedFiles.IsEnabled = enable;
        AnalyzeTooLongMethods.IsEnabled = enable;
        AnalyzeGodObjects.IsEnabled = enable;
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