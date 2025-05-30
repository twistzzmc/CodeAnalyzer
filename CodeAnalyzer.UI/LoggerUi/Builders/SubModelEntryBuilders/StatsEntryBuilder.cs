using CodeAnalyzer.Core.Models.Stats;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;

internal sealed class StatsEntryBuilder : IModelEntryBuilder<Statistics>
{
    public string Key => "Stats";
    
    public LogEntry Build(Statistics model)
    {
        return new SimpleLogEntryBuilder("Statystyki")
            .WithKey(Key)
            .WithChild($"[{model.IsCboSet}] CBO: {model.Cbo}")
            .WithChild($"[{model.IsWmpcSet}] WMPC: {model.Wmpc}")
            .WithChild($"[{model.IsFanInSet}] FanIn: {model.FanIn.FanIn}")
            .WithChild($"[{model.IsAtfdSet}] ATFD: {model.Atfd}")
            .WithChild($"[{model.IsTccSet}] TCC: {model.Tcc}")
            .Build();
    }
}