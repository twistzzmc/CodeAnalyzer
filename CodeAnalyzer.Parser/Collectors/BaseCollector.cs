using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors;

public abstract class BaseCollector<TModel, TNode>(IWarningRegistry warningRegistry) : ICollector<TModel, TNode>
    where TModel : IModel
    where TNode : CSharpSyntaxNode
{
    private IdentifierCreator IdentifierCreator { get; } = new(warningRegistry);
    protected IdentifierDto? CurrentModelIdentifier { get; private set; }
    protected abstract ModelType CollectorType { get; }

    public TModel Collect(TNode node)
    {
        try
        {
            CurrentModelIdentifier = IdentifierCreator.Create(GetName(node), node);
            warningRegistry.OnWarningNeedsContext += FillWarningContext;
            return InnerCollect(node);
        }
        finally
        {
            warningRegistry.OnWarningNeedsContext -= FillWarningContext;
            CurrentModelIdentifier = null;
        }
    }

    protected abstract TModel InnerCollect(TNode node);
    protected abstract string GetName(TNode node);

    private void FillWarningContext(object? sender, WarningEventArgs args)
    {
        if (CurrentModelIdentifier is null) throw new InvalidOperationException("Current model identifier was not set");

        args.ProvideContext(CurrentModelIdentifier, CollectorType);
    }
}