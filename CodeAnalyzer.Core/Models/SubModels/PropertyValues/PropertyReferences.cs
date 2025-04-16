using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models.SubModels.PropertyValues;

public sealed class PropertyReferences(IEnumerable<ReferenceInstance> get, IEnumerable<ReferenceInstance> set)
    : IPropertyValue<IReadOnlyList<ReferenceInstance>>
{
    public IReadOnlyList<ReferenceInstance> Get { get; } = get.ToList();
    public IReadOnlyList<ReferenceInstance> Set { get; } = set.ToList();
}