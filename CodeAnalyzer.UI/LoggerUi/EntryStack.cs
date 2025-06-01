using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi;

internal sealed class EntryStack
{
    private readonly Stack<LogEntry> _stack = [];
    
    public int Count => _stack.Count;
    
    public void Push(LogEntry entry)
    {
        if (_stack.Count > 0)
        {
            _stack.Peek().AddChild(entry);
        }
        
        _stack.Push(entry);
    }

    public bool TryPop()
    {
        return _stack.TryPop(out _);
    }

    public void Register(LogEntry entry, Collection<LogEntry> collection)
    {
        if (_stack.Count > 0)
        {
            _stack.Peek().AddChild(entry);
            return;
        }
        
        collection.Add(entry);
    }
}