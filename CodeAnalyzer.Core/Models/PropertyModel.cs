using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;

namespace CodeAnalyzer.Core.Models;

public class PropertyModel : IModel
{
    public required IdentifierDto Identifier { get; init; }
    public required AccessModifierType AccessModifierType { get; init; }
    public required ReturnType Type { get; init; }
    public required int LineStart { get; init; }
    public required bool HasSetter { get; init; }
    public required int GetLength { get; init; }
    public required int SetLength { get; init; }
    public required int GetCyclomaticComplexity { get; init; }
    public required int SetCyclomaticComplexity { get; init; }
    public required IReadOnlyList<ReferenceInstance> GetReferences { get; init; }
    public required IReadOnlyList<ReferenceInstance> SetReferences { get; init; }
}