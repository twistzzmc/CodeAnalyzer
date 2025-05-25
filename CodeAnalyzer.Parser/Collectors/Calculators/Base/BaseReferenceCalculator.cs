using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Parser.Collectors.Creators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors.Calculators.Base;

internal abstract class BaseReferenceCalculator(IWarningRegistry warningRegistry, CSharpCompilation compilation)
{
    protected delegate bool IsReferenceDelegate<in TInvocation>(
        TInvocation invocation,
        SemanticModel treeModel,
        ISymbol? memberSymbol) where TInvocation : CSharpSyntaxNode;
    
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry) { ExpectNonNamespaceDeclarations = true };

    protected IEnumerable<ReferenceInstance> BaseCalculate<TInvocation>(
        CSharpSyntaxNode node,
        IsReferenceDelegate<TInvocation> isReference)
            where TInvocation : CSharpSyntaxNode
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
        ISymbol? memberSymbol = semanticModel.GetDeclaredSymbol(node);

        if (memberSymbol is null)
        {
            return [];
        }
        
        List<ReferenceInstance> references = [];
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode root = tree.GetRoot();
            IEnumerable<TInvocation> invocations = root.DescendantNodes().OfType<TInvocation>();

            foreach (TInvocation invocation in invocations)
            {
                if (isReference(invocation, treeModel, memberSymbol))
                {
                    references.Add(CreateReference(invocation));
                }
            }
        }
        
        return references;
    }

    private ReferenceInstance CreateReference(CSharpSyntaxNode reference)
    {
        return new ReferenceInstance
        {
            Namespace = _namespaceCreator.CreateJoined(reference),
            LineNumber = reference.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
            ColumnNumber = reference.GetLocation().GetLineSpan().StartLinePosition.Character + 1
        };
    }
}