using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Calculators.Base;
using CodeAnalyzer.Parser.Collectors.Creators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class ReferencesCalculator(
    IWarningRegistry warningRegistry,
    CSharpCompilation compilation) : BasePropertyCalculator<IEnumerable<ReferenceInstance>>
{
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry) { ExpectNonNamespaceDeclarations = true };
    
    public override IEnumerable<ReferenceInstance> Calculate(CSharpSyntaxNode options)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        ISymbol? memberSymbol = semanticModel.GetDeclaredSymbol(options);
        List<ReferenceInstance> references = [];

        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode root = tree.GetRoot();
            
            IEnumerable<InvocationExpressionSyntax> invocations = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>();

            foreach (InvocationExpressionSyntax invocation in invocations)
            {
                ISymbol? invocationSymbol = ModelExtensions.GetSymbolInfo(treeModel, invocation).Symbol;
                if (SymbolEqualityComparer.Default.Equals(invocationSymbol, memberSymbol))
                {
                    references.Add(CreateReference(invocation));
                }
            }
        }

        return references;
    }

    protected override IPropertyValue<IEnumerable<ReferenceInstance>> Create(
        IEnumerable<ReferenceInstance>? get,
        IEnumerable<ReferenceInstance>? set)
    {
        return new PropertyReferences(get ?? [], set ?? []);
    }

    private ReferenceInstance CreateReference(InvocationExpressionSyntax invocation)
    {
        return new ReferenceInstance
        {
            Namespace = _namespaceCreator.CreateJoined(invocation),
            LineNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
            ColumnNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Character + 1
        };
    }
}