using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Factories;


internal sealed class CollectorFactory(IWarningRegistry warningRegistry) : ICollectorFactory
{
    public ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector()
    {
        return new MethodCollector(warningRegistry);
    }

    public ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector()
    {
        return new PropertyCollector(warningRegistry);
    }
}