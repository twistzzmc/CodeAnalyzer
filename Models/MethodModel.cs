using CodeAnalyzer.Models.Interfaces;

namespace CodeAnalyzer.Models;

public class MethodModel(string name) : IModel
{
    public string Name { get; set; } = name;
}