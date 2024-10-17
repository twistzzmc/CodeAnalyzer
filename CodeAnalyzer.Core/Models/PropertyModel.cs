using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class PropertyModel(string id, string name) : IModel
{
    public string Id => id;
    public string Name => name;
    public Type? Type { get; set; }

    public override string ToString()
    {
        return $"[{Id}] {nameof(PropertyModel)}({Name})";
    }
}