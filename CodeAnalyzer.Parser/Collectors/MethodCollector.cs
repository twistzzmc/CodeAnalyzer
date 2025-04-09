using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Creators;
using CodeAnalyzer.Parser.Converters;
using CodeAnalyzer.Parser.Guards;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector(IWarningRegistry warningRegistry, CSharpCompilation compilation)
    : BaseCollector<MethodModel, MethodDeclarationSyntax>(warningRegistry)
{
    private readonly AccessModifierConverter _accessModifierConverter = new(warningRegistry);
    private readonly NamespaceCreator _namespaceCreator = new(warningRegistry) { ExpectNonNamespaceDeclarations = true };
    private readonly ReturnTypeConverter _returnTypeConverter = new();

    protected override ModelType CollectorType => ModelType.Method;

    protected override MethodModel InnerCollect(MethodDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        ModifierGuard.GuardAgainstUnknown(warningRegistry, node.Modifiers.Select(m => m.Text));

        AccessModifierType modifier = _accessModifierConverter.Convert(node.Modifiers);
        ReturnType returnType = _returnTypeConverter.Convert(node.ReturnType);
        int startLine = node.GetLocation().GetLineSpan().StartLinePosition.Line;
        int length = CalculateMethodLength(node);
        int cyclomaticComplexity = CalculateCyclomaticComplexity(node);
        List<ReferenceInstance> references = CalculateReferences(node).ToList();

        return new MethodModel
        {
            Identifier = CurrentModelIdentifier,
            AccessModifierType = modifier,
            ReturnType = returnType,
            LineStart = startLine,
            Length = length,
            CyclomaticComplexity = cyclomaticComplexity,
            References = references
        };
    }

    protected override string GetName(MethodDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }

    private IEnumerable<ReferenceInstance> CalculateReferences(MethodDeclarationSyntax method)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
        IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(method);

        IdentifierCreator identifierCreator = new(warningRegistry);
        List<ReferenceInstance> references = [];

        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel treeModel = compilation.GetSemanticModel(tree);
            SyntaxNode root = tree.GetRoot();
            
            IEnumerable<InvocationExpressionSyntax> invocations = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>();

            foreach (InvocationExpressionSyntax invocation in invocations)
            {
                ISymbol? symbol = treeModel.GetSymbolInfo(invocation).Symbol;
                if (SymbolEqualityComparer.Default.Equals(symbol, methodSymbol))
                {
                    references.Add(new ReferenceInstance
                    {
                        Namespace = _namespaceCreator.CreateJoined(invocation),
                        LineNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        ColumnNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Character + 1
                    });
                }
            }
        }

        return references;
    }

    private int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
        try
        {
            ControlFlowGraph? cfg = ControlFlowGraph.Create(method, semanticModel);
            if (cfg is null)
            {
                IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(method);
                INamedTypeSymbol? containingType = methodSymbol?.ContainingType;
                if (containingType?.TypeKind == TypeKind.Interface)
                {
                    // Metoda interfejsu ma złożoność równą 0 i brak grafu
                    return 0;
                }

                if (methodSymbol?.IsAbstract == true)
                {
                    // Metoda abstrakcyjna nie musi mieć implementacji
                    return 0;
                }
                
                warningRegistry.RegisterWarning(WarningType.CyclomaticComplexity, 
                    "Nie udało się zbydować ControlFlowGraph");
                return 0;
            }

            int edges = 0;
            foreach (BasicBlock block in cfg.Blocks)
            {
                if (block.FallThroughSuccessor is not null) edges++;
                if (block.ConditionalSuccessor is not null) edges++;
            }
            
            int nodes = cfg.Blocks.Length;
            int complexity = edges - nodes + 2;
            return complexity;
        }
        catch (Exception ex)
        {
            warningRegistry.RegisterWarning(WarningType.CyclomaticComplexity,
                $"Nie udało się zbydować ControlFlowGraph z powodu wyjątku: {ex.Message}");
            return 0;
        }
    }

    private static int CalculateMethodLength(MethodDeclarationSyntax node)
    {
        int startLine = node.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endLine = node.GetLocation().GetLineSpan().EndLinePosition.Line;

        return endLine - startLine + 1;
    }
}