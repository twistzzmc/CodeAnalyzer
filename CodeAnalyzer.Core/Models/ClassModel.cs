using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.Stats;

namespace CodeAnalyzer.Core.Models;

public class ClassModel(
    IdentifierDto identifier,
    ClassType classType,
    IEnumerable<MethodModel> methods,
    IEnumerable<PropertyModel> properties,
    IEnumerable<FieldModel> fields)
    : IModel
{
    public IdentifierDto Identifier { get; } = identifier;
    public ClassType Type { get; } = classType;
    
    public IReadOnlyList<MethodModel> Methods => methods.ToList();
    public IReadOnlyList<PropertyModel> Properties => properties.ToList();
    public IReadOnlyList<FieldModel> Fields => fields.ToList();

    public Statistics Stats { get; } = new();
}