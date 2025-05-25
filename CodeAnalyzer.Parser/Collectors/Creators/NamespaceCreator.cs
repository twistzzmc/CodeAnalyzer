using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Creators;

internal sealed class NamespaceCreator(IWarningRegistry warningRegistry)
{
    private List<NamespacePartDto>? _parts;
    
    public bool ExpectNonNamespaceDeclarations { get; set; }
    public bool ExpectFieldNamespaceDeclarationTypes { get; set; }

    public IEnumerable<NamespacePartDto> Create(CSharpSyntaxNode node)
    {
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
                case InterfaceDeclarationSyntax interfaceDeclaration:
                    AppendInterface(interfaceDeclaration);
                    break;
                case CompilationUnitSyntax:
                    // Korzeń pliku, do zignorowania
                    break;
                case FieldDeclarationSyntax:
                case VariableDeclarationSyntax:
                    // Dla pól do zignorowania
                    if (!ExpectFieldNamespaceDeclarationTypes)
                    {
                        goto default;
                    }
                    break;

                default:
                    if (!ExpectNonNamespaceDeclarations)
                    {
                        warningRegistry.RegisterWarning(
                            WarningType.Namespace, $"Niespodziewany rodzaj namespace: {current.GetType().Name}");
                    }
                    break;
            }
        }

        return _parts.AsEnumerable();
    }

    public string CreateJoined(CSharpSyntaxNode node)
    {
        return string.Join(".", Create(node).Select(n => n.Value));
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

    private void AppendInterface(InterfaceDeclarationSyntax interfaceDeclaration)
    {
        Add(NamespacePartDto.FromPure(interfaceDeclaration.Identifier.ToString()));
    }
    
    private void Add(NamespacePartDto partDto)
    {
        _parts?.Insert(0, partDto);
    }
}