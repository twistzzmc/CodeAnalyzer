using CodeAnalyzer.Collectors.Interfaces;
using CodeAnalyzer.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Collectors;

public class PropertyCollector : ICollector<PropertyModel, PropertyDeclarationSyntax>
{
    public PropertyModel Collect(PropertyDeclarationSyntax node)
    {
        return new PropertyModel(node.Identifier.Text);
    }
}