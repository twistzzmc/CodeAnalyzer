using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class ReferencesCalculator(IWarningRegistry warningRegistry, CSharpCompilation compilation) :
    ICalculator<IEnumerable<ReferenceInstance>, MethodDeclarationSyntax>,
    ICalculator<IEnumerable<ReferenceInstance>, PropertyDeclarationSyntax>,
    ICalculator<IEnumerable<ReferenceInstance>, VariableDeclaratorSyntax>
{
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry) { ExpectNonNamespaceDeclarations = true };
    
    public IEnumerable<ReferenceInstance> Calculate(MethodDeclarationSyntax options)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        IMethodSymbol? memberSymbol = semanticModel.GetDeclaredSymbol(options);
        List<ReferenceInstance> references = [];
        
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode root = tree.GetRoot();
            
            IEnumerable<InvocationExpressionSyntax> invocations = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>();
        
            foreach (InvocationExpressionSyntax invocation in invocations)
            {
                SymbolInfo invocationInfo = treeModel.GetSymbolInfo(invocation);
                ISymbol? invocationSymbol = invocationInfo.Symbol ?? invocationInfo.CandidateSymbols.FirstOrDefault();

                if (invocationSymbol is IMethodSymbol invokedMethod &&
                    SymbolEqualityComparer.Default.Equals(invokedMethod.OriginalDefinition,
                        memberSymbol?.OriginalDefinition))
                {
                    references.Add(CreateReference(invocation));
                }
            }
        }
        
        return references;
    }

    public IEnumerable<ReferenceInstance> Calculate(PropertyDeclarationSyntax options)
    {
        SemanticModel propertySemanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        ISymbol? propertySymbol = propertySemanticModel.GetDeclaredSymbol(options);

        if (propertySymbol == null)
        {
            return [];
        }
        
        List<ReferenceInstance> references = [];
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode treeRoot = tree.GetRoot();
        
            IEnumerable<IdentifierNameSyntax> identifiers = treeRoot.DescendantNodes()
                .OfType<IdentifierNameSyntax>();

            foreach (IdentifierNameSyntax identifier in identifiers)
            {
                ISymbol? treeIdentifierSymbol = treeModel.GetSymbolInfo(identifier).Symbol;
                if (!SymbolEqualityComparer.Default.Equals(treeIdentifierSymbol, propertySymbol))
                {
                    continue;
                }
                
                references.Add(CreateReference(identifier));
            }
        }

        return references;
    }

    public IEnumerable<ReferenceInstance> Calculate(VariableDeclaratorSyntax options)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        ISymbol? variableSymbol = semanticModel.GetDeclaredSymbol(options);

        if (variableSymbol == null)
        {
            return [];
        }

        List<ReferenceInstance> references = [];

        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode root = tree.GetRoot();

            IEnumerable<IdentifierNameSyntax> identifiers = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>();

            foreach (IdentifierNameSyntax identifier in identifiers)
            {
                ISymbol? identifierSymbol = treeModel.GetSymbolInfo(identifier).Symbol;

                if (SymbolEqualityComparer.Default.Equals(identifierSymbol, variableSymbol))
                {
                    references.Add(CreateReference(identifier));
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