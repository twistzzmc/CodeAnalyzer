using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class MtlAnalyzer : IAnalyzer<MtlConfiguration, MtlParameters, MethodModel, MethodResultDto>
{
    private MtlParameters Warning => Configuration.WarningThreshold;
    private MtlParameters Problem => Configuration.ProblemThreshold;
    
    public MtlConfiguration Configuration { get; } = new MtlConfiguration
    {
        WarningThreshold = new MtlParameters(50, 5),
        ProblemThreshold = new MtlParameters(100, 10)
    };

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