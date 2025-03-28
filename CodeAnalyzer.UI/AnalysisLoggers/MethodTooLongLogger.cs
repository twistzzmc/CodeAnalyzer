using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Core.Analyzers.Dtos.Result;
using CodeAnalyzer.Core.Analyzers.Enums;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.AnalysisLoggers;

internal sealed class MethodTooLongLogger(ILoggerUi logger)
{
    private readonly ILoggerUi _logger = logger;

    public void Log(IEnumerable<MethodResultDto> results)
    {
        List<MethodResultDto> resultList = results
            .Where(result => result.IssueType == AnalysisIssueType.MethodTooLong)
            .Where(result => result.Certainty != IssueCertainty.Info)
            .ToList();
        
        if (resultList.Count == 0)
        {
            _logger.Log("Nie znaleziono zbyt długich metod");
            return;
        }
        
        _logger.Log("Znalezione metody, które są zbyt długie:");
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
        _logger.Log($"Metoda {method.Identifier.Name} jest za duża (linia: {method.LineStart}).");
        _logger.Log($"\t Liczba linii: {method.Length}");
        _logger.Log($"\t Złożoność cyklometryczna: {method.CyclomaticComplexity}");
        _logger.Log("\t Zaleca się podzielić metode na kilka mniejszych");
        _logger.Log();
    }

    private void LogWarning(MethodResultDto result)
    {
        MethodModel method = result.Model;
        _logger.Log($"Metoda {method.Identifier.Name} może być za duża (linia: {method.LineStart}).");
        _logger.Log($"\t Liczba linii: {method.Length}");
        _logger.Log($"\t Złożoność cyklometryczna: {method.CyclomaticComplexity}");
        _logger.Log("\t Podzielenie metody może pozytywnie wpłynąć na czytelność kodu");
        _logger.Log();
    }
}