using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeAnalyzer.UI.Base;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.LoggerUi.Dtos;

public sealed class LogEntry : NotifiableProperty
{
    private readonly ObservableCollection<LogEntry> _children = [];
    private bool _isExpanded = true;
    private bool _isSuccess = false;
    private string _title = string.Empty;

    public static LogEntry Empty => new("Brak danych");
    
    public string Title { get => _title; set => SetField(ref _title, value); }
    public LogPriority Priority { get; } = LogPriority.Info;
    
    public int WarningCount { get; private set; }
    public int ErrorCount { get; private set; }
    public int ExceptionCount { get; private set; }
    public bool IsSuccess { get => _isSuccess; set => SetField(ref _isSuccess, value); }

    public IReadOnlyList<LogEntry> Children => _children;
    public bool IsExpanded { get => _isExpanded; set => SetField(ref _isExpanded, value); }

    public LogEntry()
    { }

    public LogEntry(string title, params string[] children)
        : this(title, LogPriority.Info, children)
    { }

    public LogEntry(Exception exception)
        : this(exception.Message, LogPriority.Exception, exception.StackTrace ?? string.Empty)
    { }

    public LogEntry(string title, LogPriority priority, params string[] children)
    {
        Title = title;
        Priority = priority;
        foreach (string child in children)
        {
            AddChild(new LogEntry(child));
        }
    }

    public void AddChild(string title) => AddChild(new LogEntry(title));

    public void AddChild(LogEntry child)
    {
        switch (child.Priority)
        {
            case LogPriority.Info:
                break;
            case LogPriority.Warning:
                WarningCount++;
                break;
            case LogPriority.Error:
                ErrorCount++;
                break;
            case LogPriority.Exception:
                ExceptionCount++;
                break;
            case LogPriority.Success:
                IsSuccess = true;
                break;
        }
        
        _children.Add(child);
    }

    public void ClearChildren()
    {
        _children.Clear();
    }
}