using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;

internal sealed class FieldEntryBuilder : IModelEntryBuilder<FieldModel>
{
    public LogEntry Build(FieldModel model)
    {
        return new SimpleLogEntryBuilder(model.Identifier)
            .WithChild($"Modyfikatory dostępu: {model.AccessModifierType}")
            .WithChild($"Typ zwrotny: {model.Type}")
            .WithChild($"Linia początku metody: {model.LineStart}")
            .WithChild(new ReferenceEntryBuilder(ReferenceEntryBuilder.ReferenceType.Classic).Build(model.References))
            .Build();
    }
}