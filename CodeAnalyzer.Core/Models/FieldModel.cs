using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;

namespace CodeAnalyzer.Core.Models;

public sealed class FieldModel : IModel
{
    public required IdentifierDto Identifier { get; init; }
    
    public required AccessModifierType AccessModifierType { get; init; }
    public required ReturnType Type { get; init; }
    public required int LineStart { get; init; }
    public required IReadOnlyList<ReferenceInstance> References { get; init; }
}