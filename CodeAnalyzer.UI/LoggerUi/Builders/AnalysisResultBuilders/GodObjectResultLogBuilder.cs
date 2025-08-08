using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.AnalysisResultBuilders;

internal sealed class GodObjectResultLogBuilder :
    IModelEntryBuilder<GodObjectResultDto>,
    IModelEntryBuilder<IEnumerable<GodObjectResultDto?>>
{
    private enum MetricType
    {
        Constant,
        Percentile
    }
    
    private readonly StatsEntryBuilder _statisticsBuilder = new();
    private readonly ClassEntryBuilder _classEntryBuilder = new();
    private readonly MetricsEntryBuilder _metricsBuilder = new();

    private MetricType? _metricType;
    
    public string Key => "GodObjectResult";
    
    public LogEntry Build(IEnumerable<GodObjectResultDto?> source)
    {
        List<GodObjectResultDto?> sourceList =  source.ToList();
        LogEntry mainEntry = new("Wyniki analizy GodObject");

        _metricType = MetricType.Constant;
        mainEntry.AddChild(BuildForMetric(sourceList));

        _metricType = MetricType.Percentile;
        mainEntry.AddChild(BuildForMetric(sourceList));

        _metricType = null;
        return mainEntry;
    }

    public LogEntry Build(GodObjectResultDto source)
    {
        return new SimpleLogEntryBuilder($"[{source.Certainty.ToString()}] Obiekt bóg: {source.Model.Identifier.FullName}")
            .WithChildIf(_metricType != MetricType.Percentile, _metricsBuilder.Build(source.Constant))
            .WithChildIf(_metricType != MetricType.Constant, _metricsBuilder.Build(source.Percentile))
            .WithChild(_statisticsBuilder.Build(source.Stats))
            .WithChild(_classEntryBuilder.Build(source.Model))
            .Build();
    }

    private LogEntry BuildForMetric(IEnumerable<GodObjectResultDto?> source)
    {
        IEnumerable<GodObjectResultDto?> godObjectResultDtos = source.ToList();
        int problemCount = godObjectResultDtos.Count(r => GetIssue(r) == IssueCertainty.Problem);
        int warningCount = godObjectResultDtos.Count(r => GetIssue(r) == IssueCertainty.Warning);

        LogEntry mainEntry = new(GetTitle(problemCount, warningCount));
        LogEntry problemEntry = new($"Liczba znalezionych problemów: {problemCount}");
        LogEntry waringEntry = new($"Liczba znalezionych ostrzeżeń: {warningCount}");
        LogEntry top10Entry = new("Top 10 klas poniżej progu ostrzeżenia");

        foreach (GodObjectResultDto? entry in godObjectResultDtos)
        {
            IssueCertainty? issue = GetIssue(entry);
            
            if (entry is null || issue == IssueCertainty.Info)
            {
                continue;
            }

            if (issue == IssueCertainty.Problem)
            {
                problemEntry.AddChild(Build(entry));
            }
            
            if (issue == IssueCertainty.Warning)
            {
                waringEntry.AddChild(Build(entry));
            }
        }

        godObjectResultDtos
            .Where(r => r is not null)
            .Cast<GodObjectResultDto>()
            .Where(r => r.Certainty == IssueCertainty.Info)
            .OrderBy(OrderByBasedOnMetricType)
            .Take(10)
            .ToList()
            .ForEach(r => top10Entry.AddChild(Build(r)));

        mainEntry.AddChild(problemEntry);
        mainEntry.AddChild(waringEntry);
        mainEntry.AddChild(top10Entry);
        return mainEntry;
    }

    private string GetTitle(int problemCount, int warningCount)
    {
        string prefix = _metricType == MetricType.Constant
            ? "[Metryki stałe]"
            : "[Metryki zmienne]";
        
        return $"{prefix} Znalezione GodObject: {problemCount + warningCount}";
    }

    private double OrderByBasedOnMetricType(GodObjectResultDto result)
    {
        return _metricType == MetricType.Constant
            ? result.Constant.CertaintyPercent
            : result.Percentile.Score;
    }

    private IssueCertainty? GetIssue(GodObjectResultDto? result)
    {
        return _metricType == MetricType.Constant
            ? result?.Constant.Certainty
            : result?.Percentile.Certainty;
    }
}