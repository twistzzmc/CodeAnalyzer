using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CodeWalker(ICollectorFactory collectorFactory, SyntaxTree tree) : CSharpSyntaxWalker
{
    private readonly IdentifierCreator _identifierCreator = new(collectorFactory.WarningRegistry);

    public ClassModelsBuilder Builder { get; set; } = new();

    public CodeWalker(IWarningRegistry warningRegistry, SyntaxTree tree)
        : this(new CollectorFactory(warningRegistry), tree)
    { }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        IdentifierDto classIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        Builder.RegisterClass(classIdentifier);
        base.VisitClassDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodModel model = collectorFactory.CreateMethodCollector(tree).Collect(node);
        Builder.RegisterMethod(model);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        PropertyModel model = collectorFactory.CreatePropertyCollector().Collect(node);
        Builder.RegisterProperty(model);
        base.VisitPropertyDeclaration(node);
    }
}