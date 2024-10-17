using CodeAnalyzer.Models.Interfaces;

namespace CodeAnalyzer.Models;

public class PropertyModel(string name) : IModel
{
    public string Name { get; set; } = name;
    public Type? Type { get; set; }
}