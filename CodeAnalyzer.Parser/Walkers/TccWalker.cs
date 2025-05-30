using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class TccWalker
{
    private readonly Dictionary<MethodDeclarationSyntax, HashSet<IFieldSymbol>> _methodFieldAccess = new();

    private SemanticModel? _semanticModel;
    private INamedTypeSymbol? _currentClass;
    
    public MethodDeclarationSyntax? CurrentMethod { get; set; }

    public void EnterClass(SemanticModel semanticModel, INamedTypeSymbol classSymbol)
    {
        _semanticModel = semanticModel;
        _currentClass = classSymbol;
        _methodFieldAccess.Clear();
    }

    public void RegisterMethod(MethodDeclarationSyntax method)
    {
        _methodFieldAccess[method] = new HashSet<IFieldSymbol>(SymbolEqualityComparer.Default);
    }

    public void AnalyzeFieldAccess(MemberAccessExpressionSyntax node)
    {
        if (CurrentMethod is null || _semanticModel == null || !_methodFieldAccess.ContainsKey(CurrentMethod))
        {
            return;
        }

        var symbol = _semanticModel.GetSymbolInfo(node).Symbol;
        if (symbol is IFieldSymbol field &&
            SymbolEqualityComparer.Default.Equals(field.ContainingType, _currentClass))
        {
            _methodFieldAccess[CurrentMethod].Add(field);
        }
    }

    public double CalculateTcc()
    {
        var methods = _methodFieldAccess.Keys.ToList();
        int n = methods.Count;

        if (n < 2) return 1.0; // tylko 0 lub 1 metoda = pełna spójność

        int np = n * (n - 1) / 2;
        int ndc = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                var setA = _methodFieldAccess[methods[i]];
                var setB = _methodFieldAccess[methods[j]];

                if (setA.Intersect(setB).Any())
                    ndc++;
            }
        }

        return (double)ndc / np;
    }
}