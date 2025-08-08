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
    private readonly StatsEntryBuilder _statisticsBuilder = new();
    private readonly ClassEntryBuilder _classEntryBuilder = new();
    private readonly MetricsEntryBuilder _metricsBuilder = new();
    
    public string Key => "GodObjectResult";
    
    public LogEntry Build(IEnumerable<GodObjectResultDto?> source)
    {
        IEnumerable<GodObjectResultDto?> godObjectResultDtos = source.ToList();
        int problemCount = godObjectResultDtos.Count(r => r?.Certainty == IssueCertainty.Problem);
        int warningCount = godObjectResultDtos.Count(r => r?.Certainty == IssueCertainty.Warning);

        LogEntry mainEntry = new($"Znalezione GodObject: {problemCount + warningCount}");
        LogEntry problemEntry = new($"Liczba znalezionych problemów: {problemCount}");
        LogEntry waringEntry = new($"Liczba znalezionych ostrzeżeń: {warningCount}");
        LogEntry top10Entry = new("Top 10 klas poniżej progu ostrzeżenia");

        foreach (GodObjectResultDto? entry in godObjectResultDtos)
        {
            if (entry is null || entry.Certainty == IssueCertainty.Info)
            {
                continue;
            }

            if (entry.Certainty == IssueCertainty.Problem)
            {
                problemEntry.AddChild(Build(entry));
            }
            
            if (entry.Certainty == IssueCertainty.Warning)
            {
                waringEntry.AddChild(Build(entry));
            }
        }

        godObjectResultDtos
            .Where(r => r is not null)
            .Cast<GodObjectResultDto>()
            .Where(r => r.Certainty == IssueCertainty.Info)
            .Take(10)
            .ToList()
            .ForEach(r => top10Entry.AddChild(Build(r)));

        mainEntry.AddChild(problemEntry);
        mainEntry.AddChild(waringEntry);
        mainEntry.AddChild(top10Entry);
        return mainEntry;
    }

    public LogEntry Build(GodObjectResultDto source)
    {
        return new SimpleLogEntryBuilder($"[{source.Certainty.ToString()}] Obiekt bóg: {source.Model.Identifier.FullName}")
            .WithChild(_metricsBuilder.Build(source.Constant))
            .WithChild(_metricsBuilder.Build(source.Percentile))
            .WithChild(_statisticsBuilder.Build(source.Stats))
            .WithChild(_classEntryBuilder.Build(source.Model))
            .Build();
    }
}