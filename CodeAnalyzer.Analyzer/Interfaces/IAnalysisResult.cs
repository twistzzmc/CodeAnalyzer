using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Analyzer.Interfaces;

internal interface IAnalysisResult<out TModel>
    where TModel : IModel
{
    TModel Model { get; }
    AnalysisIssueType IssueType { get; }
    IssueCertainty Certainty { get; }
}