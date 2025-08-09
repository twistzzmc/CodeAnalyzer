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
    
    private readonly JsonSerializerOptions _jsonOptions;
    
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
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        _jsonOptions.Converters.Add(new LogEntryJsonConverter());
    }

    private async void OnExportResultClicked(object? sender, RoutedEventArgs e)
    {
        TopLevel? top = TopLevel.GetTopLevel(this);
        if (top is null) return;
        
        IStorageFile? file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Zapisz jako JSON",
            SuggestedFileName = "raport.json",
            FileTypeChoices =
            [
                new FilePickerFileType("JSON file")
                {
                    Patterns = ["*.json"]
                }
            ],
            DefaultExtension = "json"
        });

        if (file is null)
        {
            return;
        }
        
        await using Stream stream = await file.OpenWriteAsync();
        await JsonSerializer.SerializeAsync(stream, ResultLogger.Collect(), _jsonOptions);
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
                new("JSON") { Patterns = ["*.json"] }
            }
        };

        IReadOnlyList<IStorageFile> files = await top.StorageProvider.OpenFilePickerAsync(options);

        if (!files.Any())
            return;

        IStorageFile file = files[0];
        
        await using Stream stream = await file.OpenReadAsync();
        IEnumerable<LogEntry>? entries = await JsonSerializer.DeserializeAsync<IEnumerable<LogEntry>>(stream, _jsonOptions);

        entries?.ToList().ForEach(logEntry => ResultLogger.AddEntry(logEntry));
    }
}