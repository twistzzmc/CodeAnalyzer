using System.Collections.ObjectModel;
using CodeAnalyzer.Core.Interfaces;

namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Tight Class Cohesion
/// </summary>
public sealed class TccDto(double tcc, Dictionary<string, IEnumerable<string>> referencesInMethods)
    : IJoinable<TccDto>
{
    private readonly Dictionary<string, IEnumerable<string>> _referencesInMethods = referencesInMethods;
    
    public double Tcc => tcc;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ReferencesInMethods { get; } =
        ToReadOnlyDictionaryOfCollections(referencesInMethods);

    public static TccDto Empty => new(0, []);

    private static ReadOnlyDictionary<string, IReadOnlyCollection<string>> ToReadOnlyDictionaryOfCollections(
        Dictionary<string, IEnumerable<string>> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Dictionary<string, IReadOnlyCollection<string>> converted = source.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value as IReadOnlyCollection<string> ?? kvp.Value.ToList());

        return new ReadOnlyDictionary<string, IReadOnlyCollection<string>>(converted);
    }

    public TccDto Join(TccDto other)
    {
        if (Tcc > other.Tcc)
        {
            return new TccDto(Tcc, _referencesInMethods);
        }
        
        return new TccDto(other.Tcc, other._referencesInMethods);
    }
}