using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class MethodAnalyzer : IAnalyzer<MethodModel, MethodResultDto>
{
    public MtlConfiguration Warning { get; } = new(50, 5);
    public MtlConfiguration Problem { get; } = new(100, 10);

    public MethodResultDto Analyze(MethodModel model)
    {
        if (model.Length > Problem.LineLength && model.CyclomaticComplexity > Problem.CyclomaticComplexity)
        {
            return new MethodResultDto(model, AnalysisIssueType.MethodTooLong, IssueCertainty.Problem);
        }
        
        if (model.Length > Warning.LineLength && model.CyclomaticComplexity > Warning.CyclomaticComplexity)
        {
            return new MethodResultDto(model, AnalysisIssueType.MethodTooLong, IssueCertainty.Warning);
        }

        return new MethodResultDto(model, AnalysisIssueType.None, IssueCertainty.Info);
    }

    public IEnumerable<MethodResultDto> Analyze(IEnumerable<MethodModel> models)
    {
        return models.Select(Analyze);
    }
}