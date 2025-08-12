using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Results;

public sealed record MtcResultDto(
    MethodModel Model,
    AnalysisIssueType IssueType,
    IssueCertainty Certainty) : IAnalysisResult<MethodModel>;