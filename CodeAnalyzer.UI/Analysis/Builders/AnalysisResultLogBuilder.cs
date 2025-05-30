using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis.Builders;

internal sealed class AnalysisResultLogBuilder : IModelEntryBuilder<GodObjectResultDto>
{
    public string Key => "GodObjectResult";

    public LogEntry Build(GodObjectResultDto source)
    {
        return new LogEntry($"[{source.Certainty.ToString()}] Obiekt bóg: {source.PercentageOfUsage}");
    }
}