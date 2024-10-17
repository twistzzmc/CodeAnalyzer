using CodeAnalyzer.Core.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICollector<out TModel, in TNode>
    where TModel : IModel
    where TNode : CSharpSyntaxNode
{
    TModel Collect(TNode node);
}