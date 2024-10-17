using CodeAnalyzer.Collectors;
using CodeAnalyzer.Collectors.Interfaces;
using CodeAnalyzer.Models;
using CodeAnalyzer.Models.Builders;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Walkers;

public class CodeWalker(ICollectorFactory collectorFactory) : CSharpSyntaxWalker
{
    private readonly ClassModelBuilder _classModelBuilder = new();

    public CodeWalker()
        : this(new CollectorFactory())
    { }

    public ClassModel GetClassModel()
    {
        return _classModelBuilder.Build();
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodModel model = collectorFactory.CreateMethodCollector().Collect(node);
        _classModelBuilder.AddMethod(model);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        PropertyModel model = collectorFactory.CreatePropertyCollector().Collect(node);
        _classModelBuilder.AddProperty(model);
        base.VisitPropertyDeclaration(node);
    }
}