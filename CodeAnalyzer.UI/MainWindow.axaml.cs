using System;
using System.Net;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using CodeAnalyzer.Core.Warnings;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.UI.Controls;

namespace CodeAnalyzer.UI;

public partial class MainWindow : Window
{
    private readonly IWarningRegistry _warningRegistry;
    
    public MainWindow()
    {
        InitializeComponent();
        FilePicker.OnFileSelected += FilePickerControlOnOnFileSelected;
        _warningRegistry = new WarningRegistry();
        _warningRegistry.OnWarning += (_, data) => LogViewer.Log(data.ToString());
        Analysis.WarningRegistry = _warningRegistry;
        Analysis.FileProvider = FilePicker;
        Analysis.Logger = LogViewer;
    }

    private void FilePickerControlOnOnFileSelected(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano plik: {FilePicker.SelectedFilePath}");
    }

    private void OnAddTestLogClicked(object? sender, RoutedEventArgs e)
    {
        LogViewer.Log("To jest testowy log!");
    }
}