using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
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
            .OrderByDescending(r => r.CertaintyPercent)
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
        LogEntry entry = new($"[{source.Certainty.ToString()}] Obiekt bóg: {source.Model.Identifier.FullName}");
        entry.AddChild($"Pewność: {source.CertaintyPercent:0.00}%");
        entry.AddChild($"Czy został spełniony próg Marinescu: {source.Marinescu}");
        entry.AddChild(_statisticsBuilder.Build(source.Stats));
        entry.AddChild(_classEntryBuilder.Build(source.Model));
        return entry;
    }
}