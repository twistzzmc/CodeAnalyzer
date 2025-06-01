using CodeAnalyzer.Analyzer.Calculators;
using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using MathNet.Numerics.Statistics;

namespace CodeAnalyzer.Analyzer;

public sealed class GodObjectAnalyzer(IEnumerable<ClassModel> allClassModels)
    : IAnalyzer<GodObjectConfiguration, GodObjectParameters, ClassModel, GodObjectResultDto>
{
    private readonly FanInCalculator _fanInCalculator = new(allClassModels);
    private readonly WmpcCalculator _wmpcCalculator = new();
    
    private ClassModel? _model;
    
    private GodObjectParameters Warning => Configuration.WarningThreshold;
    private GodObjectParameters Problem => Configuration.ProblemThreshold;
    
    public GodObjectConfiguration Configuration { get; } = new GodObjectConfiguration()
    {
        ProblemThreshold = GodObjectParameters.DefaultProblem,
        WarningThreshold = GodObjectParameters.DefaultWarning
    };

    public GodObjectResultDto Analyze(ClassModel model)
    {
        _model = model;
        
        CalculateRemainingStats();
        EnsureAllStatsSet();

        double score = CalculateGodObjectScore(
            _model.Stats.Wmpc.Wmpc,
            _model.Stats.Atfd.Atfd,
            _model.Stats.Tcc.Tcc,
            _model.Stats.Cbo.Cbo,
            _model.Stats.FanIn.FanIn);

        IssueCertainty issueCertainty = score >= 80
            ? IssueCertainty.Problem
            : score >= 60 ? IssueCertainty.Warning : IssueCertainty.Info;
    
        bool isMarinescu = IsMarinescu(_model.Stats.Wmpc.Wmpc, _model.Stats.Atfd.Atfd, _model.Stats.Tcc.Tcc);
        if (!isMarinescu)
        {
            issueCertainty = IssueCertainty.Info;
        }
        
        return new GodObjectResultDto
        {
            Model = model,
            Certainty = issueCertainty,
            IssueType = AnalysisIssueType.GodObject,
            Stats = model.Stats,
            CertaintyPercent = score,
            Marinescu = isMarinescu
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
        
        if (!_model.Stats.IsWmpcSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk WMPC");
        }
        
        if (!_model.Stats.IsFanInSet)
        {
            throw new ArgumentException("Nie ustawiono statystyk FanIn");
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
        _fanInCalculator.Calculate(_model);
        _wmpcCalculator.Calculate(_model);
    }

    private static bool IsMarinescu(int wmpc, int atfd, double tcc)
    {
        return wmpc >= 47 && atfd > 5 && tcc < 0.33;
    }
    
    private static double CalculateGodObjectScore(int wmc, int atfd, double tcc, int cbo, int ca)
    {
        double normWmc  = Math.Min(1.0, wmc / 60.0);
        double normAtfd = Math.Min(1.0, atfd / 10.0);
        double normTcc  = 1.0 - Math.Min(1.0, tcc / 0.33);
        double normCbo  = Math.Min(1.0, cbo / 20.0);
        double normCa   = Math.Min(1.0, ca / 15.0);

        double score = 
            0.25 * normWmc +
            0.25 * normAtfd +
            0.20 * normTcc +
            0.15 * normCbo +
            0.15 * normCa;

        return score * 100.0;
    }
}