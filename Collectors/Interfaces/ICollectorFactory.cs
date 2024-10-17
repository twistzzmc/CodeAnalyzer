using CodeAnalyzer.Models;
using CodeAnalyzer.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Collectors.Interfaces;

public interface ICollectorFactory
{
    ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector();
    
    ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector();
}