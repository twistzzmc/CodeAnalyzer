using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;

internal sealed class ReferenceEntryBuilder(ReferenceEntryBuilder.ReferenceType type)
    : IModelEntryBuilder<IReadOnlyList<ReferenceInstance>>
{
    public enum ReferenceType
    {
        Classic,
        Get,
        Set
    }
    
    public LogEntry Build(IReadOnlyList<ReferenceInstance> model)
    {    
        SimpleLogEntryBuilder referencesBuilder = new(GetTitle(model.Count));
        model.ToList().ForEach(r => referencesBuilder.WithChild($"{r.Namespace} ({r.LineNumber})"));
        return referencesBuilder.Build();
    }

    private string GetTitle(int referenceCount)
    {
        return type switch
        {
            ReferenceType.Classic => $"[{referenceCount}] Referencje",
            ReferenceType.Get => $"[{referenceCount}] Referencje [Get]",
            ReferenceType.Set => $"[{referenceCount}] Referencje [Set]",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}