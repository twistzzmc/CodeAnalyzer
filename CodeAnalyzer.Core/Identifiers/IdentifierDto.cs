namespace CodeAnalyzer.Core.Identifiers;

public sealed class IdentifierDto(string guid, string name, IEnumerable<NamespacePartDto> parts)
{
    public string Guid { get; } = guid;
    public string Name { get; } = name;
    public IReadOnlyList<NamespacePartDto> Namespace { get; } = parts.ToList();
    public string NamespaceText => string.Join(".", Namespace.Select(n => n.Value));
    public string FullName => $"{NamespaceText}.{Name}";

    public override string ToString()
    {
        return $"[{Guid}] [{NamespaceText}] {Name}";
    }
}