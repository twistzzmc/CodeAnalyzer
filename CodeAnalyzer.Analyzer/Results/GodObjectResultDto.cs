using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Results;

public sealed record GodObjectResultDto(
    ClassModel Model,
    AnalysisIssueType IssueType,
    IssueCertainty Certainty,
    double percentageOfUsage) : IAnalysisResult<ClassModel>;