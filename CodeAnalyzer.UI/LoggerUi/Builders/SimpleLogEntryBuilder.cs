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

    public SimpleLogEntryBuilder WithChild(string title, params object[] titleParameters)
    {
        _entry.AddChild(string.Format(title, titleParameters));
        return this;
    }

    public LogEntry Build()
    {
        return _entry;
    }
}