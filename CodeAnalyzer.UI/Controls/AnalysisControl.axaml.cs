using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class AnalysisControl : UserControl
{
    public IWarningRegistry? WarningRegistry { get; set; }
    public IFileProvider? FileProvider { get; set; }
    public ILoggerUi? Logger { get; set; }
    
    public AnalysisControl()
    {
        InitializeComponent();
    }

    private void AnalyzeTooLongMethods_OnClick(object? sender, RoutedEventArgs e)
    {
        if (WarningRegistry is null)
        {
            throw new InvalidOperationException("WarningRegistry is null");
        }

        string code = File.ReadAllText(FileProvider?.Provide() ?? string.Empty);
        ClassModel model = CodeParser.Parse(WarningRegistry, code);
        Logger?.Log(model.ToString());
    }
}