using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis.Loggers;

internal sealed class GodObjectLogger(ILoggerUi logger)
{
    public void Log(IEnumerable<GodObjectResultDto?> results)
    {
        IEnumerable<GodObjectResultDto?> godObjectResultDtos = results.ToList();
        int problemCount = godObjectResultDtos.Count(r => r?.Certainty == IssueCertainty.Problem);
        int warningCount = godObjectResultDtos.Count(r => r?.Certainty == IssueCertainty.Warning);

        LogEntry mainEntry = new($"Znalezione GodObject: {problemCount + warningCount}");
        LogEntry problemEntry = new($"Liczba znalezionych problemów: {problemCount}");
        LogEntry waringEntry = new($"Liczba znalezionych ostrzeżeń: {warningCount}");

        foreach (GodObjectResultDto? entry in godObjectResultDtos)
        {
            if (entry is null || entry.Certainty == IssueCertainty.Info)
            {
                continue;
            }

            if (entry.Certainty == IssueCertainty.Problem)
            {
                problemEntry.AddChild(CreateEntry(entry));
            }
            
            if (entry.Certainty == IssueCertainty.Warning)
            {
                waringEntry.AddChild(CreateEntry(entry));
            }
        }

        mainEntry.AddChild(problemEntry);
        mainEntry.AddChild(waringEntry);
        logger.AddEntry(mainEntry);
    }

    private LogEntry CreateEntry(GodObjectResultDto entry)
    {
        LogEntry logEntry = new($"Procen użyć w innych klasach: {entry.PercentageOfUsage}");
        entry.ReferenceClasses.ToList().ForEach(rc => logEntry.AddChild(rc.Identifier.FullName));
        return logEntry;
    }
}