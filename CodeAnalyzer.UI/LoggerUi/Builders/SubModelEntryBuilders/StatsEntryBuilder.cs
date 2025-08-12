using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Core.Models.Stats;
using CodeAnalyzer.Core.Models.Stats.Data;
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
            .WithChild(BuildCbo(model.IsCboSet, model.Cbo))
            .WithChild(BuildWmpc(model.IsWmpcSet, model.Wmpc))
            .WithChild(BuildCa(model.IsCaSet, model.Ca))
            .WithChild(BuildAtfd(model.IsAtfdSet, model.Atfd))
            .WithChild(BuildTcc(model.IsTccSet, model.Tcc))
            .Build();
    }

    private LogEntry BuildCbo(bool isCboSet, CboDto cbo)
    {
        LogEntry entry = new($"[{isCboSet}] CBO: {cbo.Cbo}");
        cbo.ReferencesTypes.ToList().ForEach(rt => entry.AddChild(rt));
        return entry;
    }

    private static LogEntry BuildWmpc(bool isWmpcSet, WmpcDto wmpc)
    {
        return new LogEntry($"[{isWmpcSet}] WMPC: {wmpc.Wmpc}");
    }

    private static LogEntry BuildCa(bool isCaSet, CaDto ca)
    {
        LogEntry entry = new($"[{isCaSet}] Ca: {ca.Ca} ({ca.CaPercentage:0.00}%)");
        ca.ReferencesClassModels.ToList().ForEach(rcm => entry.AddChild(rcm.Identifier.FullName));
        return entry;
    }
    
    private static LogEntry BuildAtfd(bool isAtfdSet, AtfdDto atfd)
    {
        LogEntry entry = new($"[{isAtfdSet}] ATFD: {atfd.Atfd}");
        atfd.ReferencedSymbols.ToList().ForEach(rs => entry.AddChild(rs));
        return entry;
    }
    
    private static LogEntry BuildTcc(bool isTccSet, TccDto tcc)
    {
        LogEntry entry = new($"[{isTccSet}] TCC: {tcc.Tcc:0.00}");
        foreach (KeyValuePair<string, IReadOnlyCollection<string>> kvp in tcc.ReferencesInMethods)
        {
            LogEntry methodEntry = new(kvp.Key);
            kvp.Value.ToList().ForEach(r => methodEntry.AddChild(r));
            entry.AddChild(methodEntry);
        }
        return entry;
    }
}