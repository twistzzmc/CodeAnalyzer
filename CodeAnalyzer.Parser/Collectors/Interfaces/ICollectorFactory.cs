using CodeAnalyzer.Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICollectorFactory
{
    ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector();
    
    ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector();
}