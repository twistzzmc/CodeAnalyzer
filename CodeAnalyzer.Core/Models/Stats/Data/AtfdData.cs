namespace CodeAnalyzer.Core.Models.Stats.Data;

public sealed class AtfdData(int atfd, IEnumerable<string> referencedSymbols)
{
    public int Atfd { get; init; } = atfd;
    public IReadOnlyCollection<string> ReferencedSymbols { get; init; } = referencedSymbols.ToList();
    
    public static AtfdData Empty { get; } = new(0, []);
}