using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICollectorFactory
{
    IWarningRegistry WarningRegistry { get; }
    
    ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector(SyntaxTree tree);
    
    ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector();
}