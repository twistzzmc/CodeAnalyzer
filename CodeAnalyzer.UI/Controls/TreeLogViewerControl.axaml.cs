using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
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

    public void Success(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Success);
        // AddEntry(entry);
        _queue.Register(entry);
    }

    public void Info(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Info);
        // AddEntry(entry);
        _queue.Register(entry);
    }

    public void Warning(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Warning);
        // AddEntry(entry);
        _queue.Register(entry);
    }

    public void Error(string message, params object[] messageParameters)
    {
        LogEntry entry = new(string.Format(message, messageParameters), LogPriority.Error);
        // AddEntry(entry);
        _queue.Register(entry);
    }

    public void Exception(Exception ex)
    {
        LogEntry entry = new(ex);
        // AddEntry(entry);
        _queue.Register(entry);
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
        Dispatcher.UIThread.InvokeAsync(() => Entries.Add(entry));
        // Entries.Add(entry);
    }

    public void AddEntry(string message)
    {
        AddEntry(new LogEntry(message));
    }
    
    public void AddEntry(Exception ex)
    {
        AddEntry(new LogEntry(ex));
    }
    
    private void OnClearLogsClicked(object? sender, RoutedEventArgs e)
    {
        Entries.Clear();
    }
}