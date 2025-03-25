using CodeAnalyzer.Core.Analyzers.Dtos.Configuration;
using CodeAnalyzer.Core.Analyzers.Dtos.Result;
using CodeAnalyzer.Core.Analyzers.Enums;
using CodeAnalyzer.Core.Analyzers.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Core.Analyzers;

public sealed class MethodAnalyzer : IAnalyzer<MethodModel, MethodResultDto>
{
    public MethodTooLongLevelConfiguration Warning { get; }
        = new(50, 5);
    public MethodTooLongLevelConfiguration Problem { get; }
        = new(100, 10);

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