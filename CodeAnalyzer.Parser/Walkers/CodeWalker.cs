using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Models.Stats.Data;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CodeWalker(ICollectorFactory collectorFactory, CSharpCompilation compilation) : CSharpSyntaxWalker
{
    private readonly IdentifierCreator _identifierCreator = new(collectorFactory.WarningRegistry);
    private readonly ClassModelsBuilder _builder = new();
    private readonly CboWalker _cboWalker = new();
    private readonly AtfdWalker _atfdWalker = new();
    private readonly TccWalker _tccWalker = new();
    
    public CodeWalker(IWarningRegistry warningRegistry, CSharpCompilation compilation)
        : this(new CollectorFactory(warningRegistry), compilation)
    { }

    public IEnumerable<ClassModel> CollectClassModels()
    {
        Dictionary<IdentifierDto, CboDto> cboMap = _cboWalker.GetCboMap();
        
        IEnumerable<ClassModel> models = _builder.Build().ToList();
        foreach (ClassModel model in models)
        {
            model.Stats.Cbo = cboMap.TryGetValue(model.Identifier, out CboDto? cbo) ? cbo : CboDto.Empty;
        }

        return models;
    }
    
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
        
        _builder.RegisterClass(classIdentifier);
        
        base.VisitClassDeclaration(node);

        if (classSymbol is not null)
        {
            _builder.RegisterAtfd(classIdentifier, _atfdWalker.GetAtfd());
            _builder.RegisterTcc(classIdentifier, _tccWalker.CalculateTcc());
        }
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        IdentifierDto interfaceIdentifier = _identifierCreator.Create(node.Identifier.Text, node);
        _builder.RegisterInterface(interfaceIdentifier);
        _builder.RegisterAtfd(interfaceIdentifier, AtfdDto.Empty);
        _builder.RegisterTcc(interfaceIdentifier, TccDto.Empty);
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodModel model = collectorFactory.CreateMethodCollector(compilation).Collect(node);
        _builder.RegisterMethod(model);
        _tccWalker.CurrentMethod = node;
        
        base.VisitMethodDeclaration(node);
        
        _tccWalker.CurrentMethod = null;
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