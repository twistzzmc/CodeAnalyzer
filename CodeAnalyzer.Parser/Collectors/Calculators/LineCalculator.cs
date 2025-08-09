using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class LineCalculator : ICalculator<int, CSharpSyntaxNode>
{
    public int Calculate(CSharpSyntaxNode options)
    {
        return options.GetLocation().GetLineSpan().StartLinePosition.Line;
    }
}