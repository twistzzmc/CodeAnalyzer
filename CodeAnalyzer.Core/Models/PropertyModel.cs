using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class PropertyModel(string name) : IModel
{
    public string Name => name;
    public Type? Type { get; set; }

    public override string ToString()
    {
        return $"PropertyModel({Name})";
    }
}