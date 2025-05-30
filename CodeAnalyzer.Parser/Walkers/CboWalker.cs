using CodeAnalyzer.Core.Identifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CboWalker
{
    private readonly Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> _dependencies
        = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<IdentifierDto, INamedTypeSymbol> _classMap = new();
    
    private SemanticModel? _semanticModel;
    private INamedTypeSymbol? _currentClass;

    public Dictionary<IdentifierDto, int> GetCboMap()
    {
        return _classMap.ToDictionary(
            x => x.Key,
            x => GetCbo(x.Key));
    }

    public void EnterClass(IdentifierDto classIdentifier, SemanticModel semanticModel, INamedTypeSymbol classSymbol)
    {
        _semanticModel = semanticModel;
        _currentClass = classSymbol;

        if (!_dependencies.ContainsKey(classSymbol))
        {
            _dependencies[classSymbol] = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            _classMap[classIdentifier] = classSymbol;
        }
    }

    public void AddDependency(IdentifierNameSyntax node)
    {
        ISymbol? symbol = _semanticModel?.GetSymbolInfo(node).Symbol;
        AddDependency(symbol);
    }

    public void AddDependency(TypeSyntax typeSyntax)
    {
        if (_semanticModel == null) return;

        INamedTypeSymbol? symbol = ModelExtensions.GetSymbolInfo(_semanticModel, typeSyntax).Symbol as INamedTypeSymbol;
        AddDependency(symbol);
    }

    private void AddDependency(ISymbol? symbol)
    {
        if (symbol is IFieldSymbol or IMethodSymbol or IPropertySymbol)
        {
            AddDependency(symbol.ContainingType);
        }
    }

    private void AddDependency(INamedTypeSymbol? targetType)
    {
        if (_currentClass == null || targetType == null)
        {
            return;
        }

        if (!SymbolEqualityComparer.Default.Equals(targetType, _currentClass))
        {
            _dependencies[_currentClass].Add(targetType);
        }
    }

    private int GetCbo(IdentifierDto classIdentifier)
    {
        if (!_classMap.TryGetValue(classIdentifier, out INamedTypeSymbol? classSymbol))
        {
            return 0;
        }
        
        return _dependencies.TryGetValue(classSymbol, out HashSet<INamedTypeSymbol>? set) ? set.Count : 0;
    }
}