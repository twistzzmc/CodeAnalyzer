using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Interfaces;

namespace CodeAnalyzer.Analyzer.Configurations;

public sealed class MtcConfiguration : IAnalysisConfiguration<MtcParameters>
{
    public required MtcParameters WarningThreshold { get; init; }
    public required MtcParameters ProblemThreshold { get; init; }
}