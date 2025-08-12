namespace CodeAnalyzer.Analyzer.Results.GodObject;

public sealed class PercentileThresholds
{
    public required double AtfdP90 { get; init; }
    public required double WmcP90 { get; init; }
    public required double TccP10 { get; init; }
    public required double CboP90 { get; init; }
    public required double CaP90 { get; init; }
    

    public static PercentileThresholds Empty => new PercentileThresholds
    {
        AtfdP90 = 0,
        WmcP90 = 0,
        TccP10 = 0,
        CboP90 = 0,
        CaP90 = 0
    };
}