using System.Collections.ObjectModel;

namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Tight Class Cohesion
/// </summary>
public sealed class TccDto(double tcc, Dictionary<string, IEnumerable<string>> referencesInMethods)
{
    public double Tcc { get; init; } = tcc;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ReferencesInMethods { get; init; } =
        ToReadOnlyDictionaryOfCollections(referencesInMethods);

    public static TccDto Empty => new TccDto(0, []);
    
    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> 
        ToReadOnlyDictionaryOfCollections(Dictionary<string, IEnumerable<string>> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Dictionary<string, IReadOnlyCollection<string>> converted = source.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value as IReadOnlyCollection<string> ?? kvp.Value.ToList());

        return new ReadOnlyDictionary<string, IReadOnlyCollection<string>>(converted);
    }
}