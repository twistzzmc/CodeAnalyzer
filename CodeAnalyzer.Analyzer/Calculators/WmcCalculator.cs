using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Analyzer.Calculators;

internal sealed class WmcCalculator : IClassStatCalculator
{
    public void Calculate(ClassModel model)
    {
        int wmc = 0;
        wmc += model.Methods.Sum(m => m.CyclomaticComplexity);
        wmc += model.Properties.Sum(p => p.CyclomaticComplexity.Get + p.CyclomaticComplexity.Set);
        model.Stats.Wmc = new WmcDto(wmc);
    }
}