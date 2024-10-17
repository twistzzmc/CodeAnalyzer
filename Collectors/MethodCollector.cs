using CodeAnalyzer.Collectors.Interfaces;
using CodeAnalyzer.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Collectors;

public class MethodCollector : ICollector<MethodModel, MethodDeclarationSyntax>
{
    public MethodModel Collect(MethodDeclarationSyntax node)
    {
        return new MethodModel(node.Identifier.Text);
    }
}