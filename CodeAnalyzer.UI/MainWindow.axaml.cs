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
        FilePicker.OnFileSelected += FilePickerControlOnOnFileSelected;
        WarningRegistry = new WarningRegistry();
        WarningRegistry.OnWarning += (_, data) => LogViewer.Log(data.ToString());
        DataContext = this;
    }

    private void FilePickerControlOnOnFileSelected(object? sender, EventArgs e)
    {
        LogViewer.Log($"Wybrano plik: {FilePicker.SelectedFilePath}");
    }
}