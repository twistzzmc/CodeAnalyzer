using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class CodePickerControl : UserControl, ICodePathProvider, IAsyncErrorThrower
{
    public string SelectedFilePath { get; private set; } = string.Empty;
    public string SelectedFolderPath { get; private set; } = string.Empty;

    public CodePickerControl()
    {
        InitializeComponent();
    }

    public event EventHandler<Exception>? OnError;

    public string ProvideFile()
    {
        return SelectedFilePath;
    }

    public string ProvideFolder()
    {
        return SelectedFolderPath;
    }

    public event EventHandler? OnFileSelected;
    public event EventHandler? OnFolderSelected;

    private async void OnPickFileClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            await SelectFile();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, ex);
        }
    }

    private async void OnPickFolderClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            await SelectFolder();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, ex);
        }
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

        if (files.Any())
        {
            SelectedFilePath = files[0].Path.LocalPath;
            SelectedFolderPath = string.Empty;
            OnFileSelected?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task SelectFolder()
    {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) return;

        FolderPickerOpenOptions options = new()
        {
            Title = "Wbierz plik",
            AllowMultiple = false
        };

        IReadOnlyList<IStorageFolder> folders = await top.StorageProvider.OpenFolderPickerAsync(options);

        if (folders.Any())
        {
            SelectedFolderPath = folders[0].Path.LocalPath;
            SelectedFilePath = string.Empty;
            OnFolderSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}