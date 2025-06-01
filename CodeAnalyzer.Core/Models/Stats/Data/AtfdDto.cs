namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Access To Foreign Data
/// </summary>
public sealed class AtfdDto(int atfd, IEnumerable<string> referencedSymbols)
{
    public int Atfd { get; } = atfd;
    public IReadOnlyCollection<string> ReferencedSymbols { get; } = referencedSymbols.ToList();
    
    public static AtfdDto Empty { get; } = new(0, []);
}