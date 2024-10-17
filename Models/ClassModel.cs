namespace CodeAnalyzer.Models;

public class ClassModel(IEnumerable<MethodModel> methods, IEnumerable<PropertyModel> properties)
{
    public readonly IReadOnlyList<MethodModel> Methods = methods.ToList();
    public readonly IReadOnlyList<PropertyModel> Properties = properties.ToList();
}