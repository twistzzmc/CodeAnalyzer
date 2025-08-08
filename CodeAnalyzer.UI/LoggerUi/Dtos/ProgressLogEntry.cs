using System;
using System.Threading;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.LoggerUi.Dtos;

internal sealed class ProgressLogEntry : LogEntry, IProgress
{
    private readonly Lock _lock = new(); 
    
    private readonly uint _total;
    private uint _current;
    
    private string _originalTitle;
    private string _progressTitle = string.Empty;

    public uint Total
    {
        get { lock (_lock) return _total; }
        private init { lock (_lock) _total = value; }
    }

    public uint Current
    {
        get { lock (_lock) return _current; }
        set
        {
            lock (_lock)
            {
                _current = value;
            }
            
            SetProgressTitle();
        }
    }

    public override string Title
    {
        get => _progressTitle;
        set
        {
            _originalTitle = value;
            SetProgressTitle();
            base.Title = _progressTitle;
        }
    }

    public ProgressLogEntry(int totalProgress, string title)
        : base(title, LogPriority.NewLevel)
    {
        Total = Convert.ToUInt32(totalProgress);
        _originalTitle = title;
        SetProgressTitle();
    }

    private void SetProgressTitle()
    {
        _progressTitle = $"[{Current}/{Total}] {_originalTitle}";
        base.Title = _progressTitle;
    }
}