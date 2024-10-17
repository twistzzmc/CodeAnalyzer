using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors;

public abstract class BaseCollector<TModel, TNode>(IWarningRegistry warningRegistry) : ICollector<TModel, TNode>
    where TModel : IModel
    where TNode : CSharpSyntaxNode
{
    protected string? CurrentModelIdentifier { get; private set; }
    protected abstract ModelType CollectorType { get; }

    public TModel Collect(TNode node)
    {
        try
        {
            SetIdentifier();
            warningRegistry.OnWarningNeedsContext += FillWarningContext;
            return InnerCollect(node);
        }
        finally
        {
            warningRegistry.OnWarningNeedsContext -= FillWarningContext;
            ClearIdentifier();
        }
    }

    protected abstract TModel InnerCollect(TNode node);

    private void FillWarningContext(object? sender, WarningEventArgs args)
    {
        if (CurrentModelIdentifier is null)
        {
            throw new InvalidOperationException("Current model identifier was not set");
        }
        
        args.ProvideContext(CurrentModelIdentifier, CollectorType);
    }

    private void SetIdentifier()
    {
        CurrentModelIdentifier = IdentifierCreator.Create();
    }

    private void ClearIdentifier()
    {
        CurrentModelIdentifier = null;
    }
}