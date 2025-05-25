using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;

namespace CodeAnalyzer.Core.Models;

public class PropertyModel : IModel
{
    public required IdentifierDto Identifier { get; init; }
    public required AccessModifierType AccessModifierType { get; init; }
    public required ReturnType Type { get; init; }
    public required int LineStart { get; init; }
    public required PropertyLength Length { get; init; }
    public required PropertyCyclomaticComplexity CyclomaticComplexity { get; init; }
    public required IReadOnlyList<ReferenceInstance> References { get; init; }
    public bool HasSetter => Length.Set > 0;
}