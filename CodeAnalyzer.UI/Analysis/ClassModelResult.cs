using System.Collections.Generic;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.Analysis.Builders;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class ClassModelResult(ClassModel model)
{
    public ClassModel Model { get; } = model;
    public LogEntry ModelEntry { get; private set; } = LogEntry.Empty;
    public GodObjectResultDto? GodObjectResult { get; private set; }
    public LogEntry GodObjectEntry { get; private set; } = LogEntry.Empty;

    public void AddModelEntry(ClassEntryBuilder classEntryBuilder)
    {
        ModelEntry = classEntryBuilder.Build(Model);
    }

    public void AddGodObjectAnalysis(
        GodObjectAnalyzer godObjectAnalyzer,
        AnalysisResultLogBuilder analysisResultLogBuilder)
    {
        GodObjectResult = godObjectAnalyzer.Analyze(Model);
        GodObjectEntry = analysisResultLogBuilder.Build(GodObjectResult);
        ModelEntry.AddChild(GodObjectEntry);
    }
}