using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class MethodModel(string name) : IModel
{
    public string Name => name;

    public override string ToString()
    {
        return $"MethodModel({Name})";
    }
}