using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi;

internal sealed class EntryQueue
{
    private readonly Queue<LogEntry> _queue = [];
    
    public void Enqueue(LogEntry entry)
    {
        if (_queue.Count > 0)
        {
            _queue.Peek().AddChild(entry);
        }
        
        _queue.Enqueue(entry);
    }

    public bool TryDequeue()
    {
        return _queue.TryDequeue(out _);
    }

    public void Register(LogEntry entry, Collection<LogEntry> collection)
    {
        if (_queue.Count > 0)
        {
            _queue.Peek().AddChild(entry);
            return;
        }
        
        collection.Add(entry);
    }
}