using CodeAnalyzer.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Collectors.Interfaces;

public interface ICollector<out TModel, in TNode>
    where TModel : IModel
    where TNode : CSharpSyntaxNode
{
    TModel Collect(TNode node);
}