using CodeAnalyzer.Core.Interfaces;

namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Access To Foreign Data
/// </summary>
public sealed class AtfdDto(int atfd, IEnumerable<string> referencedSymbols) : IJoinable<AtfdDto>
{
    public int Atfd { get; } = atfd;
    public IReadOnlyCollection<string> ReferencedSymbols { get; } = referencedSymbols.ToList();
    
    public static AtfdDto Empty { get; } = new(0, []);
    
    public AtfdDto Join(AtfdDto other)
    {
        return new AtfdDto(Atfd + other.Atfd, [..ReferencedSymbols, ..other.ReferencedSymbols]);
    }
}