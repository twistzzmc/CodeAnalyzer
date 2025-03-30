using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class TreeLogViewerControl : UserControl, ILoggerUi
{
    public ObservableCollection<LogEntry> Entries { get; } = [];
    
    public TreeLogViewerControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void Log(LogEntry entry)
    {
        Entries.Add(entry);
    }

    public void Log(string message)
    {
        Entries.Add(new LogEntry(message));
    }
    
    public void Log()
    {
        Entries.Add(new LogEntry());
    }
    
    public void Log(Exception ex)
    {
        Entries.Add(new LogEntry(ex.Message, ex.StackTrace ?? string.Empty));
    }
    
    private void OnClearLogsClicked(object? sender, RoutedEventArgs e)
    {
        Entries.Clear();
    }
}