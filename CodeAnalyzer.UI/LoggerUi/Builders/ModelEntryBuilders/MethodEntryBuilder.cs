using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;

internal sealed class MethodEntryBuilder : IModelEntryBuilder<MethodModel>
{
    public LogEntry Build(MethodModel model)
    {
        return new SimpleLogEntryBuilder(model.Identifier)
            .WithChild($"Modyfikatory dostępu: {model.AccessModifierType}")
            .WithChild($"Typ zwrotny: {model.ReturnType}")
            .WithChild($"Linia początku metody: {model.LineStart}")
            .WithChild($"Długość metody: {model.Length}")
            .WithChild($"Złożoność cyklometryczna: {model.CyclomaticComplexity}")
            .Build();
    }
}