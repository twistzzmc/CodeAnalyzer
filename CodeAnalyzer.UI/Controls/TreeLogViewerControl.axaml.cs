using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.UI.LoggerUi;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.Controls;

public partial class TreeLogViewerControl : UserControl, ILoggerUi, ILogger
{
    private readonly EntryQueue _queue = new();
    
    public ObservableCollection<LogEntry> Entries { get; } = [];
    
    public TreeLogViewerControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void OpenLevel(string title, params object[] titleParameters)
    {
        LogEntry entry = new(string.Format(title, titleParameters), LogPriority.NewLevel);
        AddEntry(entry);
        _queue.Enqueue(entry);
    }

    public IProgress OpenProgress(int total, string title, params object[] titleParameters)
    {
        ProgressLogEntry progressEntry = new(total, string.Format(title, titleParameters));
        AddEntry(progressEntry);
        _queue.Enqueue(progressEntry);
        return progressEntry;
    }

    public void Success(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Success);
        _queue.Register(entry, Entries);
    }

    public void Info(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Info);
        _queue.Register(entry, Entries);
    }

    public void Info(IProgress progress, string message, params object[] messageParameters)
    {
        Info(message, messageParameters);
        progress.Current++;
    }

    public void Warning(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Warning);
        _queue.Register(entry, Entries);
    }

    public void Error(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Error);
        _queue.Register(entry, Entries);
    }

    public void Exception(Exception ex)
    {
        LogEntry entry = new(ex);
        _queue.Register(entry, Entries);
    }

    public void CloseLevel()
    {
        if (!_queue.TryDequeue())
        {
            AddEntry(new LogEntry("Próba zamknięcia poziomu logowania na najwyższym poziomie", LogPriority.Error));
        }
    }

    public void AddEntry(LogEntry entry)
    {
        Entries.Add(entry);
    }

    public void AddEntry(string message)
    {
        AddEntry(new LogEntry(message));
    }
    
    public void AddEntry(Exception ex)
    {
        AddEntry(new LogEntry(ex));
    }

    public IEnumerable<LogEntry> Collect()
    {
        return Entries;
    }

    private void OnClearLogsClicked(object? sender, RoutedEventArgs e)
    {
        Entries.Clear();
    }
}