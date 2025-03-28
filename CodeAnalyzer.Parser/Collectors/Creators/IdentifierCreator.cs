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
    private List<NamespacePartDto>? _parts;

    public IdentifierDto Create(string name, CSharpSyntaxNode node)
    {
        IdentifierDto identifier = new(CreateGuid(), name, []);
        _parts = [];

        for (SyntaxNode? current = node; current is not null; current = current.Parent)
        {
            if (current == node) continue;

            switch (current)
            {
                case ClassDeclarationSyntax classDeclaration:
                    AppendClass(classDeclaration);
                    break;
                case StructDeclarationSyntax structDeclaration:
                    AppendStruct(structDeclaration);
                    break;
                case RecordDeclarationSyntax recordDeclaration:
                    AppendRecord(recordDeclaration);
                    break;
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    AppendNamespace(namespaceDeclaration);
                    break;
                case FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration:
                    AppendNamespace(fileScopedNamespaceDeclaration);
                    break;

                default:
                    warningRegistry.RegisterWarning(
                        identifier, node.ToModelType(), WarningType.UnexpectedNamespaceMember, current.GetType().Name);
                    break;
            }
        }

        return new IdentifierDto(identifier.Guid, identifier.Name, _parts);
    }

    private void AppendClass(ClassDeclarationSyntax classDeclaration)
    {
        Add(NamespacePartDto.FromClass(classDeclaration.Identifier.Text, false, false));
    }

    private void AppendStruct(StructDeclarationSyntax structDeclaration)
    {
        Add(NamespacePartDto.FromClass(structDeclaration.Identifier.Text, false, false));
    }

    private void AppendRecord(RecordDeclarationSyntax recordDeclaration)
    {
        Add(NamespacePartDto.FromClass(recordDeclaration.Identifier.Text, false, false));
    }

    private void AppendNamespace(BaseNamespaceDeclarationSyntax namespaceDeclaration)
    {
        Add(NamespacePartDto.FromPure(namespaceDeclaration.Name.ToString()));
    }

    private void Add(NamespacePartDto partDto)
    {
        _parts?.Insert(0, partDto);
    }

    private string CreateGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
}