using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Creators;

public sealed class IdentifierCreator(IWarningRegistry warningRegistry)
{
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry);

    public IdentifierDto Create(string name, CSharpSyntaxNode node)
    {
        return new IdentifierDto(CreateGuid(), name, _namespaceCreator.Create(node));
    }

    private static string CreateGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
}