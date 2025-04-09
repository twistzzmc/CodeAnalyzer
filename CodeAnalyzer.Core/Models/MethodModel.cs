using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;

namespace CodeAnalyzer.Core.Models;

public class MethodModel : IModel
{
    public required IdentifierDto Identifier { get; init; }
    public required AccessModifierType AccessModifierType { get; init; }
    public required ReturnType ReturnType { get; init; }
    public required int LineStart { get; init; }
    public required int Length { get; init; }
    public required int CyclomaticComplexity { get; init; }
    public required IReadOnlyList<ReferenceInstance> References { get; init; }

    public override string ToString()
    {
        return $"{nameof(MethodModel)} {Identifier}, " +
               $"{nameof(AccessModifierType)}: {AccessModifierType}, " +
               $"{nameof(ReturnType)}: {ReturnType}, " +
               $"{nameof(LineStart)}: {LineStart}, " +
               $"{nameof(Length)}: {Length}, " +
               $"{nameof(CyclomaticComplexity)}: {CyclomaticComplexity})";
    }
}