using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

public class ReturnTypeCalculator : ICalculator<ReturnType, TypeSyntax>
{
    public ReturnType Calculate(TypeSyntax typeSyntax)
    {
        return new ReturnType(typeSyntax.ToString());
    }
}