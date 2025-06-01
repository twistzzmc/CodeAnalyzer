using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi.Interfaces;

internal interface IModelEntryBuilder<in TSource>
{
    string Key { get; }
    
    LogEntry Build(TSource source);
}