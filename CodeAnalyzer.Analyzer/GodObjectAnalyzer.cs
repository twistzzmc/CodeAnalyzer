using CodeAnalyzer.Analyzer.Calculators;
using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class GodObjectAnalyzer(IEnumerable<ClassModel> allClassModels)
    : IAnalyzer<GodObjectConfiguration, GodObjectParameters, ClassModel, GodObjectResultDto>
{
    private GodObjectParameters Warning => Configuration.WarningThreshold;
    private GodObjectParameters Problem => Configuration.ProblemThreshold;
    
    public GodObjectConfiguration Configuration { get; } = new GodObjectConfiguration()
    {
        ProblemThreshold = new GodObjectParameters(20),
        WarningThreshold = new GodObjectParameters(10)
    };
    
    public GodObjectResultDto Analyze(ClassModel model)
    {
        new FanInCalculator(allClassModels).Calculate(model);
        
        IssueCertainty issueCertainty = model.Stats.FanIn.FanInPercentage > Problem.PercentageOfUsage
            ? IssueCertainty.Problem
            : model.Stats.FanIn.FanInPercentage > Warning.PercentageOfUsage
                ? IssueCertainty.Warning
                : IssueCertainty.Info;

        return new GodObjectResultDto(
            model,
            AnalysisIssueType.GodObject,
            issueCertainty,
            model.Stats.FanIn.FanInPercentage,
            model.Stats.FanIn.ReferencesClassModels);
    }

    public IEnumerable<GodObjectResultDto> Analyze(IEnumerable<ClassModel> models)
    {
        return models.Select(Analyze);
    }
}