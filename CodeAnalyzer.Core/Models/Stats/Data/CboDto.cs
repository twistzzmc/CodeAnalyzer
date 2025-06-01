namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Coupling Between Objects
/// </summary>
public sealed class CboDto(int cbo, IEnumerable<string> referencesTypes)
{
    public int Cbo { get; init; } = cbo;
    public IReadOnlyCollection<string> ReferencesTypes { get; init; } = referencesTypes.ToList();

    public static CboDto Empty { get; } = new(0, []);
}