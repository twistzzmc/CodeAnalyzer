using CodeAnalyzer.Core.Models.Stats.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class TccWalker
{
    private readonly Dictionary<MethodDeclarationSyntax, HashSet<ISymbol>> _methodFieldAccess = new();

    private SemanticModel? _semanticModel;
    private INamedTypeSymbol? _currentClass;
    private MethodDeclarationSyntax? _currentMethod;

    public MethodDeclarationSyntax? CurrentMethod
    {
        get => _currentMethod;
        set
        {
            _currentMethod = value;
            if (value is not null)
            {
                RegisterMethod(value);
            }
        }
    }

    public void EnterClass(SemanticModel semanticModel, INamedTypeSymbol classSymbol)
    {
        _semanticModel = semanticModel;
        _currentClass = classSymbol;
        _methodFieldAccess.Clear();
    }
    
    public void AnalyzeFieldAccess(IdentifierNameSyntax node)
    {
        AnalyzeSyntaxNodeAccess(node);
    }

    public void AnalyzeFieldAccess(MemberAccessExpressionSyntax node)
    {
        AnalyzeSyntaxNodeAccess(node);
    }

    public TccDto CalculateTcc()
    {
        List<MethodDeclarationSyntax> methods = _methodFieldAccess.Keys.ToList();
        int n = methods.Count;

        if (n < 2)
        {
            // tylko 0 lub 1 metoda = pełna spójność
            return new TccDto(1, ToReadable());
        }

        int np = n * (n - 1) / 2;
        int ndc = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                var setA = _methodFieldAccess[methods[i]];
                var setB = _methodFieldAccess[methods[j]];

                if (setA.Intersect(setB, SymbolEqualityComparer.Default).Any())
                    ndc++;
            }
        }

        return new TccDto((double)ndc / np, ToReadable());
    }

    private Dictionary<string, IEnumerable<string>> ToReadable()
    {
        Dictionary<string, IEnumerable<string>> map = new();
        foreach (KeyValuePair<MethodDeclarationSyntax, HashSet<ISymbol>> kvp in _methodFieldAccess.Where(kvp => kvp.Value.Count != 0))
        {
            map[kvp.Key.Identifier.Text] = kvp.Value.Select(f => f.Name).ToList();
        }
        return map;
    }

    private void RegisterMethod(MethodDeclarationSyntax method)
    {
        _methodFieldAccess[method] = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
    }

    private void AnalyzeSyntaxNodeAccess(SyntaxNode node)
    {
        if (CurrentMethod is null ||
            _semanticModel == null ||
            node.SyntaxTree != _semanticModel.SyntaxTree ||
            !_methodFieldAccess.TryGetValue(CurrentMethod, out HashSet<ISymbol>? value))
        {
            return;
        }

        ISymbol? symbol = _semanticModel.GetSymbolInfo(node).Symbol;
        if (symbol is IFieldSymbol or IPropertySymbol &&
            SymbolEqualityComparer.Default.Equals(symbol.ContainingType, _currentClass))
        {
            value.Add(symbol);
        }
    }
}