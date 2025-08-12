using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;

internal sealed class MetricsEntryBuilder
    : IModelEntryBuilder<ConstantMetric>, IModelEntryBuilder<PercentileMetric>
{
    public string Key => "Metrics";

    public LogEntry Build(ConstantMetric source)
    {
        return new SimpleLogEntryBuilder($"Metryki stałe: [{source.Certainty}, {source.CertaintyPercent}]")
            .WithChild($"Pewność: {source.CertaintyPercent}")
            .WithChild($"Czy został spełniony próg Marinescu: {source.Marinescu}")
            .Build();
    }
    
    public LogEntry Build(PercentileMetric source)
    {
        return new SimpleLogEntryBuilder($"Metryki zmienne: [{source.Certainty}, {source.Score}]")
            .WithChild($"Liczba punktów: {source.Score}")
            .WithChildIf(source.IsAtfdHit, "Przekroczono próg AFTD")
            .WithChildIf(source.IsWmcHit, "Przekroczono próg WMC")
            .WithChildIf(source.IsTccHit, "Przekroczono próg TCC")
            .WithChildIf(source.IsCboHit, "Przekroczono próg CBO")
            .WithChildIf(source.IsCaHit, "Przekroczono próg Ca")
            .Build();
    }
}