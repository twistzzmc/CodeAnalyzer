using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class MtcAnalyzer : IAnalyzer<MtcConfiguration, MtcParameters, MethodModel, MtcResultDto>
{
    private MtcParameters Warning => Configuration.WarningThreshold;
    private MtcParameters Problem => Configuration.ProblemThreshold;
    
    public MtcConfiguration Configuration { get; } = new MtcConfiguration
    {
        WarningThreshold = new MtcParameters(50, 5),
        ProblemThreshold = new MtcParameters(100, 10)
    };

    public MtcResultDto Analyze(MethodModel model)
    {
        if (model.Length > Problem.LineLength && model.CyclomaticComplexity > Problem.CyclomaticComplexity)
        {
            return new MtcResultDto(model, AnalysisIssueType.MethodTooComplex, IssueCertainty.Problem);
        }
        
        if (model.Length > Warning.LineLength && model.CyclomaticComplexity > Warning.CyclomaticComplexity)
        {
            return new MtcResultDto(model, AnalysisIssueType.MethodTooComplex, IssueCertainty.Warning);
        }

        return new MtcResultDto(model, AnalysisIssueType.None, IssueCertainty.Info);
    }

    public IEnumerable<MtcResultDto> Analyze(IEnumerable<MethodModel> models)
    {
        return models.Select(Analyze);
    }
}