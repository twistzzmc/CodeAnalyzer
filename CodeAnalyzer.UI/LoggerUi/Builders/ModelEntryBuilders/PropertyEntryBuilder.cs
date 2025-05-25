using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;

internal sealed class PropertyEntryBuilder : IModelEntryBuilder<PropertyModel>
{
    public LogEntry Build(PropertyModel model)
    {
        return new SimpleLogEntryBuilder(model.Identifier)
            .WithChild($"Modyfikatory dostępu: {model.AccessModifierType}")
            .WithChild($"Typ: {model.Type}")
            .WithChild($"Linia początku metody: {model.LineStart}")
            .WithChild($"Długość metody Get: {model.Length.Get}")
            .WithChild($"Długość metody Set: {model.Length.Set}")
            .WithChild($"Złożoność cyklometryczna Get: {model.CyclomaticComplexity.Get}")
            .WithChild($"Złożoność cyklometryczna Set: {model.CyclomaticComplexity.Set}")
            .WithChild(new ReferenceEntryBuilder(ReferenceEntryBuilder.ReferenceType.Get).Build(model.References))
            .Build();
    }
}