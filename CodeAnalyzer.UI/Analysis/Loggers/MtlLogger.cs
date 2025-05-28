using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Builders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis.Loggers;

internal sealed class MtlLogger(ILoggerUi logger)
{
    private LogEntry? _mainEntry;
    
    public void Log(IEnumerable<MtlResultDto> results)
    {
        List<MtlResultDto> resultList = results
            .Where(result => result.IssueType == AnalysisIssueType.MethodTooLong)
            .Where(result => result.Certainty != IssueCertainty.Info)
            .ToList();
        
        if (resultList.Count == 0)
        {
            logger.AddEntry("Nie znaleziono zbyt długich metod");
            return;
        }

        _mainEntry = new LogEntry("Znalezione metody, które są zbyt długie:");
        logger.AddEntry(_mainEntry);
        resultList
            .Where(result => result.Certainty == IssueCertainty.Problem)
            .ToList()
            .ForEach(LogProblem);
        
        resultList
            .Where(result => result.Certainty == IssueCertainty.Warning)
            .ToList()
            .ForEach(LogWarning);
    }

    public static void Log(ILoggerUi logger, IEnumerable<MtlResultDto> results)
    {
        new MtlLogger(logger).Log(results);
    }

    private void LogProblem(MtlResultDto result)
    {
        MethodModel method = result.Model;
        LogEntry log = new SimpleLogEntryBuilder($"Metoda {method.Identifier.FullName} jest za duża")
                .WithChild($"Początkowa linia: {method.LineStart}")
                .WithChild($"Liczba linii: {method.Length}")
                .WithChild("Złożoność cyklometryczna: {method.CyclomaticComplexity}")
                .WithChild("Zaleca się podzielić metode na kilka mniejszych")
                .Build();
        _mainEntry?.AddChild(log);
    }

    private void LogWarning(MtlResultDto result)
    {
        MethodModel method = result.Model;
        LogEntry log = new SimpleLogEntryBuilder($"Metoda {method.Identifier.FullName} może być za duża")
            .WithChild($"Początkowa linia: {method.LineStart}")
            .WithChild($"Liczba linii: {method.Length}")
            .WithChild("Złożoność cyklometryczna: {method.CyclomaticComplexity}")
            .WithChild("Podzielenie metody może pozytywnie wpłynąć na czytelność kodu")
            .Build();
        _mainEntry?.AddChild(log);
    }
}