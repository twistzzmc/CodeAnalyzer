using System;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.LoggerUi.Dtos;

internal sealed class ProgressLogEntry : LogEntry, IProgress
{
    private string _originalTitle;
    private string _progressTitle = string.Empty;
    private uint _current = 0;
    
    public uint Total { get; }

    public uint Current
    {
        get => _current;
        set
        {
            _current = value;
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