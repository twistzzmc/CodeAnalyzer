using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Parser.Collectors.Calculators.Base;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class ReferencesCalculator(IWarningRegistry warningRegistry, CSharpCompilation compilation) :
    BaseReferenceCalculator(warningRegistry, compilation),
    ICalculator<IEnumerable<ReferenceInstance>, MethodDeclarationSyntax>,
    ICalculator<IEnumerable<ReferenceInstance>, PropertyDeclarationSyntax>,
    ICalculator<IEnumerable<ReferenceInstance>, VariableDeclaratorSyntax>
{

    public IEnumerable<ReferenceInstance> Calculate(MethodDeclarationSyntax options)
    {
        return BaseCalculate<InvocationExpressionSyntax>(options, IsMethodReference);
    }

    public IEnumerable<ReferenceInstance> Calculate(PropertyDeclarationSyntax options)
    {
        return BaseCalculate<IdentifierNameSyntax>(options, IsFieldOrPropertyReference);
    }

    public IEnumerable<ReferenceInstance> Calculate(VariableDeclaratorSyntax options)
    {
        return BaseCalculate<IdentifierNameSyntax>(options, IsFieldOrPropertyReference);
    }

    private static bool IsMethodReference(
        InvocationExpressionSyntax invocation,
        SemanticModel treeModel,
        ISymbol? memberSymbol)
    {
        SymbolInfo invocationInfo = treeModel.GetSymbolInfo(invocation);
        ISymbol? invocationSymbol = invocationInfo.Symbol ?? invocationInfo.CandidateSymbols.FirstOrDefault();
        
        return invocationSymbol is IMethodSymbol invokedMethod
            && SymbolEqualityComparer.Default.Equals(
                invokedMethod.OriginalDefinition, memberSymbol?.OriginalDefinition);
    }

    private static bool IsFieldOrPropertyReference(
        IdentifierNameSyntax identifier,
        SemanticModel treeModel,
        ISymbol? memberSymbol)
    {
        ISymbol? treeIdentifierSymbol = treeModel.GetSymbolInfo(identifier).Symbol;
        return SymbolEqualityComparer.Default.Equals(treeIdentifierSymbol, memberSymbol);
    }
}