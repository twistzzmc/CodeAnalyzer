using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class PropertyCollector : ICollector<PropertyModel, PropertyDeclarationSyntax>
{
    public PropertyModel Collect(PropertyDeclarationSyntax node)
    {
        return new PropertyModel(node.Identifier.Text);
    }
}