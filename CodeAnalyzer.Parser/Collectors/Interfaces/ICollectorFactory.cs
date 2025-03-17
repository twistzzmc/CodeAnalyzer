using CodeAnalyzer.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICollectorFactory
{
    ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector(SyntaxTree tree);
    
    ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector();
}