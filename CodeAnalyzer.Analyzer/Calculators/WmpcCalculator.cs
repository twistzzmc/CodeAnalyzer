using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Calculators;

internal sealed class WmpcCalculator : IClassStatCalculator
{
    public void Calculate(ClassModel model)
    {
        int wmpc = 0;
        wmpc += model.Methods.Sum(m => m.CyclomaticComplexity);
        wmpc += model.Properties.Sum(p => p.CyclomaticComplexity.Get + p.CyclomaticComplexity.Set);
        model.Stats.Wmpc = wmpc;
    }
}