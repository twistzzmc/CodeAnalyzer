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
    ICalculator<PropertyReferences, PropertyDeclarationSyntax>
{
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry) { ExpectNonNamespaceDeclarations = true };
    
    public IEnumerable<ReferenceInstance> Calculate(MethodDeclarationSyntax options)
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

    public PropertyReferences Calculate(PropertyDeclarationSyntax options)
    {
        SemanticModel propertySemanticModel = compilation.GetSemanticModel(options.SyntaxTree);
        ISymbol? propertySymbol = propertySemanticModel.GetDeclaredSymbol(options);

        if (propertySymbol == null)
        {
            return new PropertyReferences([], []);
        }
        
        List<ReferenceInstance> getReferences = [];
        List<ReferenceInstance> setReferences = [];
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
                
                if (IsSet(treeIdentifierSymbol, identifier))
                {
                    setReferences.Add(CreateReference(identifier));
                }
                else
                {
                    getReferences.Add(CreateReference(identifier));
                }
            }
        }

        return new PropertyReferences(getReferences, setReferences);
    }

    private static bool IsSet(ISymbol treeIdentifierSymbol, IdentifierNameSyntax foundIdentifier)
    {
        if (treeIdentifierSymbol is not IPropertySymbol)
        {
            return false;
        }

        SyntaxNode? parent = foundIdentifier.Parent;
        while (parent is not null)
        {
            switch (parent)
            {
                case AssignmentExpressionSyntax assignment:
                    return assignment.Left == foundIdentifier;
                case PrefixUnaryExpressionSyntax:
                case PostfixUnaryExpressionSyntax:
                    return true;
                case ArgumentSyntax:
                case ReturnStatementSyntax:
                case ConditionalExpressionSyntax:
                case BinaryExpressionSyntax:
                case MemberAccessExpressionSyntax:
                case InvocationExpressionSyntax:
                case ElementAccessExpressionSyntax:
                case ArrayRankSpecifierSyntax:
                case ParenthesizedExpressionSyntax:
                case CastExpressionSyntax:
                    return false;
            }

            parent = parent.Parent;
        }

        return false;
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