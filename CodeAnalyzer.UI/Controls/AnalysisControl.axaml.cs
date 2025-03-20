using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class AnalysisControl : UserControl
{
    public static readonly StyledProperty<IWarningRegistry> WarningRegistryProperty =
        AvaloniaProperty.Register<AnalysisControl, IWarningRegistry>(nameof(WarningRegistry));

    public static readonly StyledProperty<IFileProvider> FileProviderProperty =
        AvaloniaProperty.Register<AnalysisControl, IFileProvider>(nameof(FileProvider));

    public static readonly StyledProperty<ILoggerUi> LoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILoggerUi>(nameof(Logger));

    public IWarningRegistry WarningRegistry
    {
        get => GetValue(WarningRegistryProperty);
        set => SetValue(WarningRegistryProperty, value);
    }

    public IFileProvider FileProvider
    {
        get => GetValue(FileProviderProperty);
        set => SetValue(FileProviderProperty, value);
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
        string code = File.ReadAllText(FileProvider.Provide());
        ClassModel model = CodeParser.Parse(WarningRegistry, code);
        Logger.Log(model.ToString());
    }
}