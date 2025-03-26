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
        FilePicker.OnFileSelected += LogFilePath;
        FilePicker.OnFolderSelected += LogFolderPath;
        WarningRegistry = new WarningRegistry();
        WarningRegistry.OnWarning += (_, data) => LogViewer.Log(data.ToString());
        DataContext = this;
    }

    private void LogFolderPath(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano folder: {FilePicker.SelectedFolderPath}");
    }

    private void LogFilePath(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano plik: {FilePicker.SelectedFilePath}");
    }
}