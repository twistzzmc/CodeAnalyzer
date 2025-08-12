using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats;

namespace CodeAnalyzer.Analyzer.Calculators;

internal sealed class PercentileCalculator(IEnumerable<ClassModel> allClassModels)
{
    private readonly Dictionary<StatType, List<double>> _percentiles = new();

    private bool IsFilled => _percentiles.Count != 0;
    
    public double GetPercentile(StatType statType, double percentile)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentile, 1, nameof(percentile));
        ArgumentOutOfRangeException.ThrowIfLessThan(percentile, 0, nameof(percentile));

        if (!IsFilled)
        {
            FillPercentiles();
        }
        
        return CalculatePercentile(statType, percentile);
    }

    private void FillPercentiles()
    {
        List<Statistics> stats = allClassModels
            .Where(c => c.Type == ClassType.Class)
            .Select(c => c.Stats)
            .ToList();

        _percentiles[StatType.AccessToFieldData] = GetSinglePercentileList(stats, s => s.Atfd.Atfd);
        _percentiles[StatType.CouplingBetweenObjects] = GetSinglePercentileList(stats, s => s.Cbo.Cbo);
        _percentiles[StatType.AfferentCoupling] = GetSinglePercentileList(stats, s => s.Ca.Ca);
        _percentiles[StatType.TightClassCohesion] = GetSinglePercentileList(stats, s => s.Tcc.Tcc);
        _percentiles[StatType.WeightedMethodsPerClass] = GetSinglePercentileList(stats, s => s.Wmpc.Wmpc);
    }
    
    private double CalculatePercentile(StatType statType, double percentile)
    {
        List<double> percentiles = _percentiles[statType];

        if (percentiles.Count == 0)
        {
            return 0;
        }
        
        double position = (percentiles.Count + 1) * percentile;
        int index = (int)position;
        double frac = position - index;

        if (index <= 0)
        {
            return percentiles[0];
        }

        if (index >= percentiles.Count)
        {
            return percentiles[^1];
        }
        
        return percentiles[index - 1] + frac * (percentiles[index] - percentiles[index - 1]);
    }

    private static List<double> GetSinglePercentileList(
        List<Statistics> classStatistics,
        Func<Statistics, double> selectStatFunc)
    {
        return classStatistics
            .Select(selectStatFunc)
            .Order()
            .ToList();
    }
}