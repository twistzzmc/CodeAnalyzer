using CodeAnalyzer.Parser.Dtos;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.OtherEntryBuilders;

internal sealed class FileEntryBuilder : IModelEntryBuilder<FileDto>
{
    public string Key => nameof(FileDto);
    
    public LogEntry Build(FileDto source)
    {
        return new LogEntry($"Plik: {source.Name} — {source.LineCount} linii, {source.CharCount} znaków");
    }
}