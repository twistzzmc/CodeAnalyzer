using System.Collections.Generic;
using CodeAnalyzer.UI.Base;

namespace CodeAnalyzer.UI.LoggerUi.Dtos;

public sealed class LogEntry : NotifiableProperty
{
    private bool _isExpanded = true;
    
    public string Title { get; set; } = string.Empty;
    public List<LogEntry> Children { get; set; } = [];
    public bool IsExpanded { get => _isExpanded; set => SetField(ref _isExpanded, value); }

    public LogEntry()
    { }

    public LogEntry(string title, params string[] children)
    {
        Title = title;
        foreach (string child in children)
        {
            Children.Add(new LogEntry(child));
        }
    }

    public void AddChild(string title) => AddChild(new LogEntry(title));
    
    public void AddChild(LogEntry child) => Children.Add(child);
}