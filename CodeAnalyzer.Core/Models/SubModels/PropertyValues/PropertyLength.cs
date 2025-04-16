using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models.SubModels.PropertyValues;

public sealed class PropertyLength(int get, int set) : IPropertyValue<int>
{
    public int Get { get; } = get;
    public int Set { get; } = set;
}