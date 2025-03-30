using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;

internal sealed class PropertyEntryBuilder : IModelEntryBuilder<PropertyModel>
{
    public LogEntry Build(PropertyModel model)
    {
        return new SimpleLogEntryBuilder(model.Identifier)
            .WithChild("Type: {0}", model.Type is null ? "<brak>" : model.Type)
            .Build();
    }
}