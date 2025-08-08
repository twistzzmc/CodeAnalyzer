using CodeAnalyzer.Analyzer.Enums;

namespace CodeAnalyzer.Analyzer.Results.GodObject;

public sealed class ConstantMetric
{
    public required IssueCertainty Certainty { get; init; }
    public required double CertaintyPercent { get; init; }
    public required bool Marinescu { get; init; }
}