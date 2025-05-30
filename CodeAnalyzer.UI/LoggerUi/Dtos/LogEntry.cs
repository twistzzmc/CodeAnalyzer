using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeAnalyzer.UI.Base;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.LoggerUi.Dtos;

public class LogEntry : NotifiableProperty
{
    private readonly ObservableCollection<LogEntry> _children = [];
    private bool _isExpanded = true;
    private bool _isSuccess;
    private int _warningCount;
    private int _errorCount;
    private int _exceptionCount;
    private int _errorOrExceptionCount;
    private bool _hasWarning;
    private bool _hasError;
    private bool _hasException;
    private bool _hasErrorOrException;
    private string _title = string.Empty;

    public static LogEntry Empty => new("Brak danych");
    
    public string Key { get; set; } = string.Empty;
    
    public virtual string Title { get => _title; set => SetField(ref _title, value); }
    public LogPriority Priority { get; } = LogPriority.Info;
    
    public int WarningCount { get => _warningCount; private set => SetField(ref _warningCount, value); }
    public int ErrorCount { get => _errorCount; private set => SetField(ref _errorCount, value); }
    public int ExceptionCount { get => _exceptionCount; private set => SetField(ref _exceptionCount, value); }
    public int ErrorOrExceptionCount { get => _errorOrExceptionCount; private set => SetField(ref _errorOrExceptionCount, value); }
    public bool IsSuccess { get => _isSuccess; set => SetField(ref _isSuccess, value); }
    
    public bool HasWarning { get => _hasWarning; private set => SetField(ref _hasWarning, value); }
    public bool HasError { get => _hasError; private set => SetField(ref _hasError, value); }
    public bool HasException { get => _hasException; private set => SetField(ref _hasException, value); }
    public bool HasErrorOrException { get => _hasErrorOrException; private set => SetField(ref _hasErrorOrException, value); }

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
                HasWarning = true;
                break;
            case LogPriority.Error:
                ErrorCount++;
                ErrorOrExceptionCount++;
                HasError = true;
                HasErrorOrException = true;
                break;
            case LogPriority.Exception:
                ExceptionCount++;
                ErrorOrExceptionCount++;
                HasException = true;
                HasErrorOrException = true;
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

    public bool RemoveChild(string key)
    {
        LogEntry? child = _children.FirstOrDefault(c => c.Key == key);
        if (child is null)
        {
            return false;
        }

        _children.Remove(child);
        return true;
    }

    public bool RemoveChildren(params string[] keys)
    {
        return keys.Aggregate(false, (current, key) => current || RemoveChild(key));
    }
}