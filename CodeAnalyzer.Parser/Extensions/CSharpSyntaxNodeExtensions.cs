using CodeAnalyzer.Core.Logging.Enums;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Extensions;

internal static class CSharpSyntaxNodeExtensions
{
    public static ModelType ToModelType(this CSharpSyntaxNode node)
    {
        switch (node)
        {
            case ClassDeclarationSyntax:
                return ModelType.Class;
            case StructDeclarationSyntax:
                return ModelType.Struct;
            case RecordDeclarationSyntax:
                return ModelType.Record;
            case MethodDeclarationSyntax:
                return ModelType.Method;
            case PropertyDeclarationSyntax:
                return ModelType.Property;
            
            default:
                throw new ArgumentException("Nieznany type modelu");
        }
    }
}