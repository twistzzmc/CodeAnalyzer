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

public sealed class GodObjectAnalyzer
    : IAnalyzer<GodObjectConfiguration, GodObjectParameters, ClassModel, GodObjectResultDto>
{
    private readonly IEnumerable<ClassModel> _classes;
    private readonly FanInCalculator _fanInCalculator;
    private readonly WmpcCalculator _wmpcCalculator;
    
    private ClassModel? _model;
    private int _problemScore;
    private int _warningScore;
    private bool _isFanInPreprocessed;
    private double _fanInMedian;
    
    private GodObjectParameters Warning => Configuration.WarningThreshold;
    private GodObjectParameters Problem => Configuration.ProblemThreshold;
    
    public GodObjectConfiguration Configuration { get; } = new GodObjectConfiguration()
    {
        ProblemThreshold = GodObjectParameters.DefaultProblem,
        WarningThreshold = GodObjectParameters.DefaultWarning
    };

    public GodObjectAnalyzer(IEnumerable<ClassModel> allClassModels)
    {
        _classes = allClassModels;
        _fanInCalculator = new FanInCalculator(_classes);
        _wmpcCalculator = new WmpcCalculator();
    }
    
    public GodObjectResultDto Analyze(ClassModel model)
    {
        _model = model;
        _problemScore = 0;
        _warningScore = 0;
        
        CalculateRemainingStats();
        EnsureAllStatsSet();
    
        Score(_model.Stats.Wmpc.Wmpc, Warning.Wmpc, Problem.Wmpc);
        Score(_model.Stats.Atfd.Atfd, Warning.Atfd, Problem.Atfd);
        Score(_model.Stats.Cbo.Cbo, Warning.Cbo, Problem.Cbo);
        Score(_model.Stats.Tcc.Tcc, Warning.Tcc, Problem.Tcc, false);
        ScoreFanIn();

        IssueCertainty issueCertainty = _problemScore >= 3
            ? IssueCertainty.Problem
            : _warningScore >= 3 ? IssueCertainty.Warning : IssueCertainty.Info;

        return new GodObjectResultDto
        {
            Model = model,
            Certainty = issueCertainty,
            IssueType = AnalysisIssueType.GodObject,
            Stats = model.Stats,
            FanInMedian = _fanInMedian
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
        PreprocessFanIn();
        _wmpcCalculator.Calculate(_model);
    }

    private void ScoreFanIn()
    {
        ArgumentNullException.ThrowIfNull(_model);
        double warningThreshold = Warning.FanInMedian * _fanInMedian;
        double problemThreshold = Problem.FanInMedian * _fanInMedian;
        Score(_model.Stats.FanIn.FanIn, warningThreshold, problemThreshold);
    }

    private void Score<T>(T value, T warningThreshold, T problemThreshold, bool expectLesser = true)
        where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(_model);
        int breachValue = expectLesser ? 1 : -1;
        if (value.CompareTo(warningThreshold) == breachValue)
        {
            WarningBreached();
        }
        
        if (value.CompareTo(problemThreshold) == breachValue)
        {
            ProblemBreached();
        }
    }

    private void PreprocessFanIn()
    {
        if (_isFanInPreprocessed)
        {
            return;
        }

        _isFanInPreprocessed = true;
        foreach (ClassModel model in _classes)
        {
            _fanInCalculator.Calculate(model);
        }

        _fanInMedian = _classes
            .Where(m => m.Type == ClassType.Class)
            .Select(m => (double)m.Stats.FanIn.FanIn)
            .Median();
    }

    private void WarningBreached()
    {
        _warningScore++;
    }

    private void ProblemBreached()
    {
        _problemScore++;
        _warningScore++;
    }
}