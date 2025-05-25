using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Interfaces;

namespace CodeAnalyzer.Analyzer.Configurations;

public sealed class GodObjectConfiguration : IAnalysisConfiguration<GodObjectParameters>
{
    public required GodObjectParameters WarningThreshold { get; init; }
    public required GodObjectParameters ProblemThreshold { get; init; }
}