using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.UI.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.Analysis.Builders;

internal sealed class AnalysisResultLogBuilder : ILogBuilder<GodObjectResultDto>
{
    public LogEntry Build(GodObjectResultDto source)
    {
        return new LogEntry($"[{source.Certainty.ToString()}] Obiekt bóg: {source.percentageOfUsage}");
    }
}