using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class FilePickerControl : UserControl, IFileProvider
{
    public event EventHandler? OnFileSelected;
    
    public string SelectedFilePath { get; private set; } = string.Empty;

    public FilePickerControl()
    {
        InitializeComponent();
    }

    public string Provide()
    {
        return SelectedFilePath;
    }

    private void OnPickFileClicked(object? sender, RoutedEventArgs e)
    {
        SelectFile();
    }

    private async Task SelectFile()
    {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) return;

        FilePickerOpenOptions options = new()
        {
            Title = "Wbierz plik",
            AllowMultiple = false
        };

        IReadOnlyList<IStorageFile> files = await top.StorageProvider.OpenFilePickerAsync(options);
        SelectedFilePath = files.Any() ? files[0].Path.LocalPath : string.Empty;
        OnFileSelected?.Invoke(this, EventArgs.Empty);
    }
}