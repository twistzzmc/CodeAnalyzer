namespace CodeAnalyzer.Analyzer.Configurations.Dtos;

public sealed record MtcParameters(
    int LineLength,
    int CyclomaticComplexity);