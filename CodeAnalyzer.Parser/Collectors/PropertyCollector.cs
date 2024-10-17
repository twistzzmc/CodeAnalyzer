using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class PropertyCollector(IWarningRegistry warningRegistry)
    : BaseCollector<PropertyModel, PropertyDeclarationSyntax>(warningRegistry)
{
    protected override ModelType CollectorType => ModelType.Property;
    
    protected override PropertyModel InnerCollect(PropertyDeclarationSyntax node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(CurrentModelIdentifier);

        return new PropertyModel(CurrentModelIdentifier, node.Identifier.Text);
    }
}