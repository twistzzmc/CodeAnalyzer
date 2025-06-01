using CodeAnalyzer.Core.Models.Stats.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class AtfdWalker
{
    private SemanticModel? _semanticModel;
    private INamedTypeSymbol? _currentClass;

    private readonly HashSet<ISymbol> _foreignAccesses = new(SymbolEqualityComparer.Default);

    public void EnterClass(SemanticModel semanticModel, INamedTypeSymbol classSymbol)
    {
        _semanticModel = semanticModel;
        _currentClass = classSymbol;
        _foreignAccesses.Clear();
    }

    public void AnalyzeMemberAccess(MemberAccessExpressionSyntax node)
    {
        if (_semanticModel == null || _currentClass == null)
        {
            return;
        }
        ISymbol? symbol = ModelExtensions.GetSymbolInfo(_semanticModel, node).Symbol;

        if (symbol is not (IFieldSymbol or IPropertySymbol))
        {
            return;
        }
        
        INamedTypeSymbol? owner = symbol.ContainingType;
        if (!SymbolEqualityComparer.Default.Equals(owner, _currentClass))
        {
            _foreignAccesses.Add(symbol);
        }
    }

    public AtfdDto GetAtfd()
    {
        return new AtfdDto(
            _foreignAccesses.Count, 
            _foreignAccesses.Select(fa => fa.ToDisplayString()));
    }
}