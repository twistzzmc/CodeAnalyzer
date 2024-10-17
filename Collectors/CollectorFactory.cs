using CodeAnalyzer.Collectors.Interfaces;
using CodeAnalyzer.Models;
using CodeAnalyzer.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Collectors;

public class CollectorFactory : ICollectorFactory
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