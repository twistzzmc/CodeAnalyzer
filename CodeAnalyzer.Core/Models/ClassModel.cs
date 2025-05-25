using System.Text;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class ClassModel(
    IdentifierDto identifier,
    IEnumerable<MethodModel> methods,
    IEnumerable<PropertyModel> properties,
    IEnumerable<FieldModel> fields)
    : IModel
{
    public IdentifierDto Identifier { get; } = identifier;
    public IReadOnlyList<MethodModel> Methods => methods.ToList();
    public IReadOnlyList<PropertyModel> Properties => properties.ToList();
    public IReadOnlyList<FieldModel> Fields => fields.ToList();
}