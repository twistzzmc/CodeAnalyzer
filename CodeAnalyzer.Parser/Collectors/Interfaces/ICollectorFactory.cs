using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICollectorFactory
{
    IWarningRegistry WarningRegistry { get; }
    
    ICollector<MethodModel, MethodDeclarationSyntax> CreateMethodCollector(CSharpCompilation compilation);
    
    ICollector<PropertyModel, PropertyDeclarationSyntax> CreatePropertyCollector(CSharpCompilation compilation);
    
    ICollector<FieldModel, VariableDeclaratorSyntax> CreateFieldCollector(
        CSharpCompilation compilation,
        FieldDeclarationSyntax field);
}