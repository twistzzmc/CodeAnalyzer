using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Factories;

internal sealed class CollectorFactory(IWarningRegistry warningRegistry) : ICollectorFactory
{
    public IWarningRegistry WarningRegistry { get; } = warningRegistry;

    public ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector(SyntaxTree tree)
    {
        return new MethodCollector(WarningRegistry, tree);
    }

    public ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector()
    {
        return new PropertyCollector(WarningRegistry);
    }
}