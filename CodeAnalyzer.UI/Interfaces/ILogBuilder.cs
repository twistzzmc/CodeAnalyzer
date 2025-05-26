using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.Interfaces;

internal interface ILogBuilder<in TSource>
{
    LogEntry Build(TSource source);
}