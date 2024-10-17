using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Factories;


internal sealed class CollectorFactory : ICollectorFactory
{
    public ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector()
    {
        return new MethodCollector();
    }

    public ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector()
    {
        return new PropertyCollector();
    }
}