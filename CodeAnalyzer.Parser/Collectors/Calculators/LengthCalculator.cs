using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;
using CodeAnalyzer.Parser.Collectors.Calculators.Base;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

internal sealed class LengthCalculator : BasePropertyCalculator<int>
{
    public override int Calculate(CSharpSyntaxNode options)
    {
        int startLine = options.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endLine = options.GetLocation().GetLineSpan().EndLinePosition.Line;

        return endLine - startLine + 1;
    }

    protected override IPropertyValue<int> Create(int get, int set)
    {
        return new PropertyLength(get, set);
    }
}