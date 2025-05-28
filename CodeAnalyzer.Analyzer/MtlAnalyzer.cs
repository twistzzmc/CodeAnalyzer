using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class MtlAnalyzer : IAnalyzer<MtlConfiguration, MtlParameters, MethodModel, MtlResultDto>
{
    private MtlParameters Warning => Configuration.WarningThreshold;
    private MtlParameters Problem => Configuration.ProblemThreshold;
    
    public MtlConfiguration Configuration { get; } = new MtlConfiguration
    {
        WarningThreshold = new MtlParameters(50, 5),
        ProblemThreshold = new MtlParameters(100, 10)
    };

    public MtlResultDto Analyze(MethodModel model)
    {
        if (model.Length > Problem.LineLength && model.CyclomaticComplexity > Problem.CyclomaticComplexity)
        {
            return new MtlResultDto(model, AnalysisIssueType.MethodTooLong, IssueCertainty.Problem);
        }
        
        if (model.Length > Warning.LineLength && model.CyclomaticComplexity > Warning.CyclomaticComplexity)
        {
            return new MtlResultDto(model, AnalysisIssueType.MethodTooLong, IssueCertainty.Warning);
        }

        return new MtlResultDto(model, AnalysisIssueType.None, IssueCertainty.Info);
    }

    public IEnumerable<MtlResultDto> Analyze(IEnumerable<MethodModel> models)
    {
        return models.Select(Analyze);
    }
}