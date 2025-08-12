using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;

internal sealed class PercentileThresholdsBuilder : IModelEntryBuilder<PercentileThresholds>
{
    public string Key => "PercentileThresholds";
    
    public LogEntry Build(PercentileThresholds source)
    {
        return new SimpleLogEntryBuilder("Progi dla zmiennych metryk")
            .WithChild($"ATFD P90: {source.AtfdP90}")
            .WithChild($"WMPC P90: {source.WmpcP90}")
            .WithChild($"TCC P10: {source.TccP10}")
            .WithChild($"CBO P90: {source.CboP90}")
            .WithChild($"Ca P90: {source.CaP90}")
            .Build();
    }
}