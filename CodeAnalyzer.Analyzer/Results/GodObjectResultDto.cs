using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Stats;

namespace CodeAnalyzer.Analyzer.Results;

public sealed class GodObjectResultDto : IAnalysisResult<ClassModel>
{
    public required ClassModel Model { get; init; }
    public required AnalysisIssueType IssueType { get; init; }
    public required IssueCertainty Certainty { get; init; }
    public required Statistics Stats { get; init; }
    public required double CertaintyPercent { get; init; }
    public required bool Marinescu { get; init; }
}