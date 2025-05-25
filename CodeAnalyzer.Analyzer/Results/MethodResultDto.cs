using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Results;

public sealed record MethodResultDto(
    MethodModel Model,
    AnalysisIssueType IssueType,
    IssueCertainty Certainty) : IAnalysisResult<MethodModel>;