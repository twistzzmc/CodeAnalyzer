using System;
using Avalonia.Controls;
using CodeAnalyzer.Core.Logging;
using CodeAnalyzer.Core.Logging.Data;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Builders;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI;

public partial class MainWindow : Window
{
    public IWarningRegistry WarningRegistry { get; }

    public MainWindow()
    {
        InitializeComponent();
        CodePicker.OnFileSelected += LogCodeFilesPath;
        CodePicker.OnFolderSelected += LogFolderPath;
        CodePicker.OnError += (_, exception) => ActiveLogViewer.AddEntry(exception);
        WarningRegistry = new WarningRegistry();
        WarningRegistry.OnWarning += LogWarning;
        DataContext = this;
    }

    private void LogFolderPath(object? sender, EventArgs e)
    {
        ActiveLogViewer.AddEntry($"Wybrano folder: {CodePicker.SelectedFolderPath}");
    }

    private void LogCodeFilesPath(object? sender, EventArgs e)
    {
        ActiveLogViewer.AddEntry($"Wybrano plik: {CodePicker.SelectedFilePath}");
    }

    private void LogWarning(object? sender, WarningData e)
    {
        LogEntry log = new SimpleLogEntryBuilder($"[Ostrzeżenie] {e.Identifier?.FullName}")
            .WithChild($"Typ modelu: {e.ModelType}")
            .WithChild($"Typ ostrzeżenia: {e.WarningType}")
            .WithChild($"Wiadomość: {e.Message}")
            .Build();
        ActiveLogViewer.AddEntry(log);
    }
}