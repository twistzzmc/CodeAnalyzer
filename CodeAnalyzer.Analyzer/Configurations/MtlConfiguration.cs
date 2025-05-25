using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Interfaces;

namespace CodeAnalyzer.Analyzer.Configurations;

public sealed class MtlConfiguration : IAnalysisConfiguration<MtlParameters>
{
    public required MtlParameters WarningThreshold { get; init; }
    public required MtlParameters ProblemThreshold { get; init; }
}