using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Builders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.AnalysisLoggers;

internal sealed class MethodTooLongLogger(ILoggerUi logger)
{
    public void Log(IEnumerable<MethodResultDto> results)
    {
        List<MethodResultDto> resultList = results
            .Where(result => result.IssueType == AnalysisIssueType.MethodTooLong)
            .Where(result => result.Certainty != IssueCertainty.Info)
            .ToList();
        
        if (resultList.Count == 0)
        {
            logger.Log("Nie znaleziono zbyt długich metod");
            return;
        }
        
        logger.Log("Znalezione metody, które są zbyt długie:");
        resultList
            .Where(result => result.Certainty == IssueCertainty.Problem)
            .ToList()
            .ForEach(LogProblem);
        
        resultList
            .Where(result => result.Certainty == IssueCertainty.Warning)
            .ToList()
            .ForEach(LogWarning);
    }

    public static void Log(ILoggerUi logger, IEnumerable<MethodResultDto> results)
    {
        new MethodTooLongLogger(logger).Log(results);
    }

    private void LogProblem(MethodResultDto result)
    {
        MethodModel method = result.Model;
        LogEntry log = new SimpleLogEntryBuilder($"Metoda {method.Identifier.FullName} jest za duża")
                .WithChild($"Początkowa linia: {method.LineStart}")
                .WithChild($"Liczba linii: {method.Length}")
                .WithChild("Złożoność cyklometryczna: {method.CyclomaticComplexity}")
                .WithChild("Zaleca się podzielić metode na kilka mniejszych")
                .Build();
        logger.Log(log);
    }

    private void LogWarning(MethodResultDto result)
    {
        MethodModel method = result.Model;
        LogEntry log = new SimpleLogEntryBuilder($"Metoda {method.Identifier.FullName} może być za duża")
            .WithChild($"Początkowa linia: {method.LineStart}")
            .WithChild($"Liczba linii: {method.Length}")
            .WithChild("Złożoność cyklometryczna: {method.CyclomaticComplexity}")
            .WithChild("Podzielenie metody może pozytywnie wpłynąć na czytelność kodu")
            .Build();
        logger.Log(log);
    }
}