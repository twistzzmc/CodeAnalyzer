using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Factories;

internal sealed class CollectorFactory(IWarningRegistry warningRegistry) : ICollectorFactory
{
    public IWarningRegistry WarningRegistry { get; } = warningRegistry;

    public ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector(CSharpCompilation compilation)
    {
        return new MethodCollector(WarningRegistry, compilation);
    }

    public ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector(CSharpCompilation compilation)
    {
        return new PropertyCollector(WarningRegistry, compilation);
    }
}