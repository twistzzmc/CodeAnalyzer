using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CodeWalker(ICollectorFactory collectorFactory, CSharpCompilation compilation) : CSharpSyntaxWalker
{
    private readonly IdentifierCreator _identifierCreator = new(collectorFactory.WarningRegistry);
    private readonly ClassModelsBuilder _builder = new();
    
    public CodeWalker(IWarningRegistry warningRegistry, CSharpCompilation compilation)
        : this(new CollectorFactory(warningRegistry), compilation)
    { }

    public IEnumerable<ClassModel> CollectClassModels()
    {
        return _builder.Build();
    }
    
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        IdentifierDto classIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        _builder.RegisterClass(classIdentifier);
        base.VisitClassDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        IdentifierDto classIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        _builder.RegisterClass(classIdentifier);
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodModel model = collectorFactory.CreateMethodCollector(compilation).Collect(node);
        _builder.RegisterMethod(model);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        PropertyModel model = collectorFactory.CreatePropertyCollector(compilation).Collect(node);
        _builder.RegisterProperty(model);
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        ICollector<FieldModel, VariableDeclaratorSyntax> collector =
            collectorFactory.CreateFieldCollector(compilation, node);
        
        foreach (VariableDeclaratorSyntax variableDeclaration in node.Declaration.Variables)
        {
            FieldModel model = collector.Collect(variableDeclaration);
            _builder.RegisterField(model);
        }
        
        base.VisitFieldDeclaration(node);
    }
}