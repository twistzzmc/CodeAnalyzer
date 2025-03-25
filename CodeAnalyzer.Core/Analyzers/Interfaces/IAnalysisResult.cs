using CodeAnalyzer.Core.Analyzers.Enums;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Analyzers.Interfaces;

internal interface IAnalysisResult<out TModel>
    where TModel : IModel
{
    TModel Model { get; }
    AnalysisIssueType IssueType { get; }
    IssueCertainty Certainty { get; }
}