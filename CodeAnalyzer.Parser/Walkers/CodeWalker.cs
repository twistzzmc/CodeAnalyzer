using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CodeWalker(ICollectorFactory collectorFactory) : CSharpSyntaxWalker
{
    private readonly ClassModelBuilder _classModelBuilder = new();

    public CodeWalker(IWarningRegistry warningRegistry)
        : this(new CollectorFactory(warningRegistry))
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