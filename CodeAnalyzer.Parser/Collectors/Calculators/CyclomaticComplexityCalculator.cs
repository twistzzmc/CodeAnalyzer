using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class CyclomaticComplexityCalculator(
    IWarningRegistry warningRegistry,
    CSharpCompilation compilation) : ICalculator<int, MemberDeclarationSyntax>
{
    public int Calculate(MemberDeclarationSyntax options)
    {
        try
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(options.SyntaxTree);
            ControlFlowGraph? cfg = ControlFlowGraph.Create(options, semanticModel);

            if (cfg is not null)
            {
                return CalculateCyclomaticComplexity(cfg);
            }
            
            HandleNullGraph(semanticModel, options);
            return 0;
        }
        catch (Exception ex)
        {
            warningRegistry.RegisterWarning(WarningType.CyclomaticComplexity,
                $"Nie udało się obliżyć złożoności cyklomatycznej z powodu wyjątku: {ex.Message}");
            return 0;
        }
    }

    private void HandleNullGraph(SemanticModel semanticModel, MemberDeclarationSyntax options)
    {
        ISymbol? methodSymbol = semanticModel.GetDeclaredSymbol(options);
        INamedTypeSymbol? containingType = methodSymbol?.ContainingType;
        
        if (containingType?.TypeKind == TypeKind.Interface)
        {
            // Metoda interfejsu ma złożoność równą 0 i brak grafu
            return;
        }

        if (methodSymbol?.IsAbstract == true)
        {
            // Metoda abstrakcyjna nie musi mieć implementacji
            return;
        }
                
        warningRegistry.RegisterWarning(WarningType.CyclomaticComplexity, 
            "Nie udało się zbydować ControlFlowGraph");
    }
    
    private static int CalculateCyclomaticComplexity(ControlFlowGraph cfg)
    {
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
}