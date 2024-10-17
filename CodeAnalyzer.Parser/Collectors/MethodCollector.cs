using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector : ICollector<MethodModel, MethodDeclarationSyntax>
{
    public MethodModel Collect(MethodDeclarationSyntax node)
    {
        return new MethodModel(node.Identifier.Text);
    }
}