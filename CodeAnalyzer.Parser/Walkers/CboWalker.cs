using CodeAnalyzer.Core.Identifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Walkers;

internal sealed class CboWalker
{
    private readonly Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> _dependencies
        = new(SymbolEqualityComparer.Default);

    private readonly Dictionary<IdentifierDto, INamedTypeSymbol> _classMap
        = new();

    private SemanticModel? _semanticModel;
    private INamedTypeSymbol? _currentClass;

    public Dictionary<IdentifierDto, int> GetCboMap()
    {
        return _classMap.ToDictionary(
            entry => entry.Key,
            entry => GetCbo(entry.Key));
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
        if (_semanticModel == null || node.SyntaxTree != _semanticModel.SyntaxTree)
        {
            return;
        }

        ISymbol? symbol = _semanticModel.GetSymbolInfo(node).Symbol;
        AddDependency(symbol);
    }

    public void AddDependency(TypeSyntax typeSyntax)
    {
        if (_semanticModel == null || typeSyntax.SyntaxTree != _semanticModel.SyntaxTree)
            return;

        INamedTypeSymbol? symbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as INamedTypeSymbol;
        AddDependency(symbol);
    }

    private void AddDependency(ISymbol? symbol)
    {
        if (symbol is IFieldSymbol or IMethodSymbol or IPropertySymbol)
        {
            AddDependency(symbol.ContainingType);
        }
        else if (symbol is INamedTypeSymbol namedType)
        {
            AddDependency(namedType);
        }
    }

    private void AddDependency(INamedTypeSymbol? targetType)
    {
        if (_currentClass == null || targetType == null)
            return;

        if (!SymbolEqualityComparer.Default.Equals(targetType, _currentClass))
        {
            _dependencies[_currentClass].Add(targetType);
        }
    }

    private int GetCbo(IdentifierDto classIdentifier)
    {
        return _classMap.TryGetValue(classIdentifier, out INamedTypeSymbol? classSymbol) &&
               _dependencies.TryGetValue(classSymbol, out HashSet<INamedTypeSymbol>? set)
            ? set.Count
            : 0;
    }
}
