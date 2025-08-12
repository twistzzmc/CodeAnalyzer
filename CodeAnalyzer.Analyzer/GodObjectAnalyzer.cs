using CodeAnalyzer.Analyzer.Calculators;
using CodeAnalyzer.Analyzer.Calculators.GodObject;
using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class GodObjectAnalyzer(List<ClassModel> allClassModels)
    : IAnalyzer<GodObjectConfiguration, GodObjectParameters, ClassModel, GodObjectResultDto>
{
    private readonly CaCalculator _caCalculator = new(allClassModels);
    private readonly WmcCalculator _wmcCalculator = new();
    private readonly ConstantIssueCalculator _constantIssueCalculator = new();
    private readonly PercentileIssueCalculator _percentileIssueCalculator = new(allClassModels);
    
    private ClassModel? _model;
    
    private GodObjectParameters Warning => Configuration.WarningThreshold;
    private GodObjectParameters Problem => Configuration.ProblemThreshold;

    public PercentileThresholds Thresholds => _percentileIssueCalculator.Thresholds;
    
    public GodObjectConfiguration Configuration { get; } = new()
    {
        ProblemThreshold = GodObjectParameters.DefaultProblem,
        WarningThreshold = GodObjectParameters.DefaultWarning
    };

    public void PreAnalysis()
    {
        foreach (ClassModel classModel in allClassModels)
        {
            _model = classModel;
            
            CalculateRemainingStats();
            EnsureAllStatsSet();
        }

        _model = null;
        _percentileIssueCalculator.CalculatePercentileThresholds();
    }

    public GodObjectResultDto Analyze(ClassModel model)
    {
        _model = model;
        
        ConstantMetric constantMetric = _constantIssueCalculator.Calculate(_model);
        PercentileMetric percentileMetric = _percentileIssueCalculator.Calculate(_model);
        
        IssueCertainty issueCertainty = (IssueCertainty)Math.Max(
            (int)constantMetric.Certainty, (int)percentileMetric.Certainty);
        
        return new GodObjectResultDto
        {
            Model = model,
            Certainty = issueCertainty,
            IssueType = AnalysisIssueType.GodObject,
            Stats = model.Stats,
            Constant = constantMetric,
            Percentile = percentileMetric
        };
    }

    public IEnumerable<GodObjectResultDto> Analyze(IEnumerable<ClassModel> models)
    {
        return models.Select(Analyze);
    }

    private void EnsureAllStatsSet()
    {
        ArgumentNullException.ThrowIfNull(_model);
        if (!_model.Stats.IsAtfdSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk ATFD");
        }
        
        if (!_model.Stats.IsWmcSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk WMC");
        }
        
        if (!_model.Stats.IsCaSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk Ca");
        }
        
        if (!_model.Stats.IsCboSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk CBO");
        }
        
        if (!_model.Stats.IsTccSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk TCC");
        }
    }

    private void CalculateRemainingStats()
    {
        ArgumentNullException.ThrowIfNull(_model);
        _caCalculator.Calculate(_model);
        _wmcCalculator.Calculate(_model);
    }
}