using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class LineCalculator : ICalculator<int, MemberDeclarationSyntax>
{
    public int Calculate(MemberDeclarationSyntax options)
    {
        return options.GetLocation().GetLineSpan().StartLinePosition.Line;
    }
}