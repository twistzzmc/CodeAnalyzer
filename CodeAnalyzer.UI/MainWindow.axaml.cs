using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.Core.Warnings;
using CodeAnalyzer.Core.Warnings.Interfaces;

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
        WarningRegistry.OnWarning += (_, data) => LogViewer.Log(data.ToString());
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
}