using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.Core.Warnings;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Interfaces;
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
        CodePicker.OnError += (_, exception) => LogViewer.Log(exception);
        WarningRegistry = new WarningRegistry();
        WarningRegistry.OnWarning += LogWarning;
        DataContext = this;
    }

    private void LogFolderPath(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano folder: {CodePicker.SelectedFolderPath}");
    }

    private void LogCodeFilesPath(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano plik: {CodePicker.SelectedFilePath}");
    }

    private void LogWarning(object? sender, WarningData e)
    {
        LogEntry log = new SimpleLogEntryBuilder($"[Ostrzeżenie] {e.Identifier}")
            .WithChild($"Typ modelu: {e.ModelType}")
            .WithChild($"Typ ostrzeżenia: {e.WarningType}")
            .WithChild($"Wiadomość: {e.Message}")
            .Build();
        LogViewer.Log(log);
    }
}