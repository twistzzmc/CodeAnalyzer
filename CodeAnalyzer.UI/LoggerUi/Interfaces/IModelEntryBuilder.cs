using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi.Interfaces;

internal interface IModelEntryBuilder<in TModel>
    where TModel : IModel
{
    LogEntry Build(TModel model);
}