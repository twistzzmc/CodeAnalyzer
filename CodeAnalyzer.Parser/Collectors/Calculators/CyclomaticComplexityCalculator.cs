using CodeAnalyzer.Core.Logging.Enums;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;
using CodeAnalyzer.Parser.Collectors.Calculators.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class CyclomaticComplexityCalculator(
    IWarningRegistry warningRegistry,
    CSharpCompilation compilation) : BasePropertyCalculator<int>
{
    public override int Calculate(CSharpSyntaxNode options)
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

    protected override IPropertyValue<int> Create(int get, int set)
    {
        return new PropertyCyclomaticComplexity(get, set);
    }

    private int HandleNullGraph(SemanticModel semanticModel, CSharpSyntaxNode options)
    {
        if (options is AccessorDeclarationSyntax)
        {
            return 1;
        }
        
        ISymbol? methodSymbol = semanticModel.GetDeclaredSymbol(options);
        INamedTypeSymbol? containingType = methodSymbol?.ContainingType;
        
        if (containingType?.TypeKind == TypeKind.Interface)
        {
            // Metoda interfejsu ma złożoność równą 0 i brak grafu
            return 0;
        }

        if (methodSymbol?.IsAbstract == true || methodSymbol?.IsExtern == true)
        {
            // Metoda abstrakcyjna lub "extern" nie musi mieć implementacji
            return 0;
        }
                
        warningRegistry.RegisterWarning(WarningType.CyclomaticComplexity, 
            "Nie udało się zbydować ControlFlowGraph");
        return 0;
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