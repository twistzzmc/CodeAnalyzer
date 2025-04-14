using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class PropertyCollector(
    IWarningRegistry warningRegistry,
    ICalculator<int, MemberDeclarationSyntax> cyclomaticComplexityCalculator,
    ICalculator<IEnumerable<ReferenceInstance>, MemberDeclarationSyntax> referencesCalculator,
    ICalculator<int, MemberDeclarationSyntax> lengthCalculator,
    ICalculator<int, MemberDeclarationSyntax> lineCalculator,
    ICalculator<AccessModifierType, SyntaxTokenList> accessModifierCalculator,
    ICalculator<ReturnType, TypeSyntax> returnTypeCalculator)
    : BaseCollector<PropertyModel, PropertyDeclarationSyntax>(warningRegistry)
{
    protected override ModelType CollectorType => ModelType.Property;

    protected override PropertyModel InnerCollect(PropertyDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        return new PropertyModel
        {
            Identifier = CurrentModelIdentifier,
            AccessModifierType = accessModifierCalculator.Calculate(node.Modifiers),
            Type = returnTypeCalculator.Calculate(node.Type),
            LineStart = lineCalculator.Calculate(node),
            Len
        };
    }

    protected override string GetName(PropertyDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }
}