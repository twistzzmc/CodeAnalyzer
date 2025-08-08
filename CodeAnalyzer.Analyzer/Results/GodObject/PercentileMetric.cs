using CodeAnalyzer.Analyzer.Enums;

namespace CodeAnalyzer.Analyzer.Results.GodObject;

public sealed class PercentileMetric
{
    public required IssueCertainty Certainty { get; init; }
    
    public required double Score { get; init; }
    
    public required bool IsAtfdHit { get; init; }
    public required bool IsWmpcHit { get; init; }
    public required bool IsTccHit { get; init; }
    public required bool IsCboHit { get; init; }
    public required bool IsFanInHit { get; init; }
}