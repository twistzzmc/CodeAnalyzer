using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;

namespace CodeAnalyzer.Analyzer.Calculators.GodObject;

internal sealed class PercentileIssueCalculator(List<ClassModel> allClassModels)
{
    private const double THRESHOLD_WARNING = 0.7;
    private const double THRESHOLD_PROBLEM = 1;
    private const double WEIGHT_ATFD = 0.4;
    private const double WEIGHT_WMPC = 0.4;
    private const double WEIGHT_TCC = 0.4;
    private const double WEIGHT_CBO = 0.2;
    private const double WEIGHT_FAN_IN = 0.2;

    private double _score;

    public PercentileThresholds Thresholds { get; private set; } = PercentileThresholds.Empty;

    public void CalculatePercentileThresholds()
    {
        PercentileCalculator percentileCalculator = new(allClassModels);

        Thresholds = new PercentileThresholds
        {
            AtfdP90 = percentileCalculator.GetPercentile(StatType.AccessToFieldData, 0.9),
            WmpcP90 = percentileCalculator.GetPercentile(StatType.WeightedMethodsPerClass,0.9),
            TccP10 = percentileCalculator.GetPercentile(StatType.TightClassCohesion,0.1),
            CboP90 = percentileCalculator.GetPercentile(StatType.CouplingBetweenObjects,0.9),
            FanInP90 = percentileCalculator.GetPercentile(StatType.AfferentCoupling,0.9)
        };
    }

    public PercentileMetric Calculate(ClassModel model)
    {
        _score = 0;

        bool isAtfdHigh = model.Stats.Atfd.Atfd > Thresholds.AtfdP90;
        bool isWmpcHigh = model.Stats.Wmpc.Wmpc > Thresholds.WmpcP90;
        bool isTccLow = model.Stats.Tcc.Tcc < Thresholds.TccP10;
        bool isCboHigh = model.Stats.Cbo.Cbo > Thresholds.CboP90;
        bool isFanInHigh = model.Stats.FanIn.FanIn > Thresholds.FanInP90;

        CalcScore(isAtfdHigh, WEIGHT_ATFD);
        CalcScore(isWmpcHigh, WEIGHT_WMPC);
        CalcScore(isTccLow, WEIGHT_TCC);
        CalcScore(isCboHigh, WEIGHT_CBO);
        CalcScore(isFanInHigh, WEIGHT_FAN_IN);
        
        IssueCertainty issueCertainty = _score >= THRESHOLD_PROBLEM
            ? IssueCertainty.Problem
            : _score >= THRESHOLD_WARNING
                ? IssueCertainty.Warning
                : IssueCertainty.Info;

        return new PercentileMetric
        {
            Certainty = issueCertainty,
            Score = _score,
            IsAtfdHit = isAtfdHigh,
            IsWmpcHit = isWmpcHigh,
            IsTccHit = isTccLow,
            IsCboHit = isCboHigh,
            IsFanInHit = isFanInHigh,
        };
    }

    private void CalcScore(bool isBad, double weight)
    {
        if (isBad)
        {
            _score += weight;
        }
    }
}