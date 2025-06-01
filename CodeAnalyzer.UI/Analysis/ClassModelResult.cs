using System.Collections.Generic;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Stats;
using CodeAnalyzer.UI.LoggerUi.Builders.AnalysisResultBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class ClassModelResult(
    ClassModel model,
    IModelEntryBuilder<GodObjectResultDto> godObjectResultBuilder,
    IModelEntryBuilder<Statistics> statisticsBuilder)
{
    public ClassModel Model { get; } = model;
    public LogEntry ModelEntry { get; private set; } = LogEntry.Empty;
    public GodObjectResultDto? GodObjectResult { get; private set; }
    public LogEntry GodObjectEntry { get; private set; } = LogEntry.Empty;

    public ClassModelResult(ClassModel model)
        : this(model, new GodObjectResultLogBuilder(), new StatsEntryBuilder())
    { }

    public void AddModelEntry(ClassEntryBuilder classEntryBuilder)
    {
        ModelEntry = classEntryBuilder.Build(Model);
    }

    public void AddGodObjectAnalysis(GodObjectAnalyzer godObjectAnalyzer)
    {
        GodObjectResult = godObjectAnalyzer.Analyze(Model);
        GodObjectEntry = godObjectResultBuilder.Build(GodObjectResult);
        LogEntry newStatsEntry = statisticsBuilder.Build(model.Stats);

        ModelEntry.RemoveChildren(godObjectResultBuilder.Key, statisticsBuilder.Key);
        
        ModelEntry.AddChild(newStatsEntry);
        ModelEntry.AddChild(GodObjectEntry);
    }
}