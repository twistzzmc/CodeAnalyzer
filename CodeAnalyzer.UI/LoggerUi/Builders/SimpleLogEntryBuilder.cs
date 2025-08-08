using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi.Builders;

internal sealed class SimpleLogEntryBuilder(string title)
{
    private readonly LogEntry _entry = new(title);

    public SimpleLogEntryBuilder(IdentifierDto identifier)
        : this(identifier.ToString())
    { }

    public SimpleLogEntryBuilder WithChild(string title)
    {
        _entry.AddChild(title);
        return this;
    }

    public SimpleLogEntryBuilder WithChildIf(bool shouldAdd, string title)
    {
        return WithChildIf(shouldAdd, new LogEntry(title));
    }

    public SimpleLogEntryBuilder WithChildIf(bool shouldAdd, LogEntry entry)
    {
        if (!shouldAdd)
        {
            return this;
        }
        
        _entry.AddChild(entry);
        return this;
    }

    public SimpleLogEntryBuilder WithChild(string title, params object[] titleParameters)
    {
        _entry.AddChild(string.Format(title, titleParameters));
        return this;
    }

    public SimpleLogEntryBuilder WithChild(LogEntry entry)
    {
        _entry.AddChild(entry);
        return this;
    }

    public SimpleLogEntryBuilder WithKey(string key)
    {
        _entry.Key = key;
        return this;
    }

    public LogEntry Build()
    {
        return _entry;
    }
}