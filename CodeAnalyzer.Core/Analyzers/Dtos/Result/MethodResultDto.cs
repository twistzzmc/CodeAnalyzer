using CodeAnalyzer.Core.Analyzers.Enums;
using CodeAnalyzer.Core.Analyzers.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Core.Analyzers.Dtos.Result;

public sealed record MethodResultDto(
    MethodModel Model,
    AnalysisIssueType IssueType,
    IssueCertainty Certainty) : IAnalysisResult<MethodModel>;