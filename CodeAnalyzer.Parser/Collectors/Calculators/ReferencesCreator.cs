using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class ReferencesCreator(
    IWarningRegistry warningRegistry,
    CSharpCompilation compilation)
    : ICalculator<IEnumerable<ReferenceInstance>, MemberDeclarationSyntax>
{
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry);
    
    public IEnumerable<ReferenceInstance> Calculate(MemberDeclarationSyntax options)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        ISymbol? memberSymbol = ModelExtensions.GetDeclaredSymbol(semanticModel, options);
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