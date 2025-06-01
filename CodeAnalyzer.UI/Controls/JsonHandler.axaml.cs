using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;
using CodeAnalyzer.UI.Tools.Json;

namespace CodeAnalyzer.UI.Controls;

public partial class JsonHandler : UserControl
{
    public static readonly StyledProperty<ILoggerUi> ResultLoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILoggerUi>(nameof(ResultLogger));

    public static readonly StyledProperty<ILogger> LoggerProperty =
        AvaloniaProperty.Register<AnalysisControl, ILogger>(nameof(Logger));
    
    public ILoggerUi ResultLogger
    {
        get => GetValue(ResultLoggerProperty);
        set => SetValue(ResultLoggerProperty, value);
    }
    
    public ILogger Logger
    {
        get => GetValue(LoggerProperty);
        set => SetValue(LoggerProperty, value);
    }
    
    public JsonHandler()
    {
        InitializeComponent();
    }

    private async void OnExportResultClicked(object? sender, RoutedEventArgs e)
    {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) return;
        
        var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Zapisz jako JSON",
            SuggestedFileName = "raport.json",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON file")
                {
                    Patterns = new[] { "*.json" }
                }
            },
            DefaultExtension = "json"
        });

        if (file is not null)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new LogEntryJsonConverter());
            await using Stream stream = await file.OpenWriteAsync();
            await JsonSerializer.SerializeAsync(stream, ResultLogger.Collect(), options);
        }
    }

    private async void OnImportResultClicked(object? sender, RoutedEventArgs e)
    {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) return;

        FilePickerOpenOptions options = new()
        {
            Title = "Wybierz plik JSON",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("JSON") { Patterns = new[] { "*.json" } }
            }
        };

        IReadOnlyList<IStorageFile> files = await top.StorageProvider.OpenFilePickerAsync(options);

        if (!files.Any())
            return;

        IStorageFile file = files[0];

        var jsonoptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        jsonoptions.Converters.Add(new LogEntryJsonConverter());
        using var stream = await file.OpenReadAsync();
        IEnumerable<LogEntry>? entries = await JsonSerializer.DeserializeAsync<IEnumerable<LogEntry>>(stream, jsonoptions);

        entries?.ToList().ForEach(e => ResultLogger.AddEntry(e));
    }
}