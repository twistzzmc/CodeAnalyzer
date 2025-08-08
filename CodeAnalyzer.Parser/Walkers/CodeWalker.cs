using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Models.Stats.Data;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CodeWalker(
    ICollectorFactory collectorFactory,
    CSharpCompilation compilation,
    ClassModelsBuilder builder) : CSharpSyntaxWalker
{
    private readonly IdentifierCreator _identifierCreator = new(collectorFactory.WarningRegistry);
    private readonly CboWalker _cboWalker = new();
    private readonly AtfdWalker _atfdWalker = new();
    private readonly TccWalker _tccWalker = new();

    public Dictionary<IdentifierDto, CboDto> CboMap => _cboWalker.GetCboMap();
    
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        IdentifierDto classIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        
        SemanticModel semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
        INamedTypeSymbol? classSymbol = semanticModel.GetDeclaredSymbol(node);

        if (classSymbol is not null)
        {
            _cboWalker.EnterClass(classIdentifier, semanticModel, classSymbol);
            _atfdWalker.EnterClass(semanticModel, classSymbol);
            _tccWalker.EnterClass(semanticModel, classSymbol);
        }
        
        builder.RegisterClass(classIdentifier);
        
        base.VisitClassDeclaration(node);

        if (classSymbol is null)
        {
            return;
        }
        
        builder.RegisterAtfd(classIdentifier, _atfdWalker.GetAtfd());
        builder.RegisterTcc(classIdentifier, _tccWalker.CalculateTcc());
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        IdentifierDto interfaceIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        builder.RegisterInterface(interfaceIdentifier);
        builder.RegisterAtfd(interfaceIdentifier, AtfdDto.Empty);
        builder.RegisterTcc(interfaceIdentifier, TccDto.Empty);
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        IdentifierDto structIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        SemanticModel semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
        INamedTypeSymbol? classSymbol = semanticModel.GetDeclaredSymbol(node);

        if (classSymbol is not null)
        {
            _cboWalker.EnterClass(structIdentifier, semanticModel, classSymbol);
            _atfdWalker.EnterClass(semanticModel, classSymbol);
            _tccWalker.EnterClass(semanticModel, classSymbol);
        }
        
        builder.RegisterStruct(structIdentifier);
        
        base.VisitStructDeclaration(node);

        if (classSymbol is null)
        {
            return;
        }
        
        builder.RegisterAtfd(structIdentifier, _atfdWalker.GetAtfd());
        builder.RegisterTcc(structIdentifier, _tccWalker.CalculateTcc());
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodModel model = collectorFactory.CreateMethodCollector(compilation).Collect(node);
        builder.RegisterMethod(model);
        _tccWalker.CurrentMethod = node;
        
        base.VisitMethodDeclaration(node);
        
        _tccWalker.CurrentMethod = null;
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        PropertyModel model = collectorFactory.CreatePropertyCollector(compilation).Collect(node);
        builder.RegisterProperty(model);
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        ICollector<FieldModel, VariableDeclaratorSyntax> collector =
            collectorFactory.CreateFieldCollector(compilation, node);
        
        foreach (VariableDeclaratorSyntax variableDeclaration in node.Declaration.Variables)
        {
            FieldModel model = collector.Collect(variableDeclaration);
            builder.RegisterField(model);
        }
        
        base.VisitFieldDeclaration(node);
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        _cboWalker.AddDependency(node.Type);
        base.VisitObjectCreationExpression(node);
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        _cboWalker.AddDependency(node);
        _tccWalker.AnalyzeFieldAccess(node);
        base.VisitIdentifierName(node);
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        _atfdWalker.AnalyzeMemberAccess(node);
        _tccWalker.AnalyzeFieldAccess(node);
        base.VisitMemberAccessExpression(node);
    }
}