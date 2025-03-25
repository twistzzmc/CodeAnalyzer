namespace CodeAnalyzer.Core.Analyzers.Dtos.Configuration;

public sealed record MethodTooLongLevelConfiguration(
    int LineLength,
    int CyclomaticComplexity);