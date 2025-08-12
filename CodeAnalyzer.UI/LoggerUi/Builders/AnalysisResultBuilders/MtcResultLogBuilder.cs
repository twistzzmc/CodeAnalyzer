using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.AnalysisResultBuilders;

internal sealed class MtcResultLogBuilder : IModelEntryBuilder<IEnumerable<MtcResultDto?>>
{
    private LogEntry? _mainEntry;
    
    public string Key => "MtcResult";
    
    public LogEntry Build(IEnumerable<MtcResultDto?> source)
    {
        List<MtcResultDto> resultList = source
            .Where(r => r is not null)
            .Cast<MtcResultDto>()
            .Where(r => r.IssueType == AnalysisIssueType.MethodTooLong)
            .Where(r => r.Certainty != IssueCertainty.Info)
            .ToList();
        
        if (resultList.Count == 0)
        {
            return new LogEntry("Nie znaleziono zbyt długich metod");
        }

        _mainEntry = new LogEntry("Znalezione metody, które są zbyt długie:");
        resultList.ForEach(AddWarningOrProblem);
        return _mainEntry;
    }

    private void AddWarningOrProblem(MtcResultDto result)
    {
        MethodModel method = result.Model;

        string title = result.Certainty == IssueCertainty.Problem
            ? $"Metoda {method.Identifier.FullName} jest za duża"
            : $"Metoda {method.Identifier.FullName} może być za duża";
        string recommendations = result.Certainty == IssueCertainty.Problem
            ? "Zaleca się podzielić metode na kilka mniejszych"
            : "Podzielenie metody może pozytywnie wpłynąć na czytelność kodu";
        
        LogEntry child = new SimpleLogEntryBuilder(title)
            .WithChild($"Początkowa linia: {method.LineStart}")
            .WithChild($"Liczba linii: {method.Length}")
            .WithChild("Złożoność cyklometryczna: {method.CyclomaticComplexity}")
            .WithChild(recommendations)
            .Build();
        
        _mainEntry?.AddChild(child);
    }
}