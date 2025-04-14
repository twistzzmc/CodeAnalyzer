using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class LengthCalculator : ICalculator<int, MemberDeclarationSyntax>
{
    public int Calculate(MemberDeclarationSyntax options)
    {
        int startLine = options.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endLine = options.GetLocation().GetLineSpan().EndLinePosition.Line;

        return endLine - startLine + 1;
    }
}