using System.Text;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class ClassModel(
    IdentifierDto identifier,
    IEnumerable<MethodModel> methods,
    IEnumerable<PropertyModel> properties)
    : IModel
{
    private static readonly string ListDelimeter = $"{Environment.NewLine}\t\t";

    public IdentifierDto Identifier { get; } = identifier;
    public IReadOnlyList<MethodModel> Methods => methods.ToList();
    public IReadOnlyList<PropertyModel> Properties => properties.ToList();

    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"{nameof(ClassModel)} {Identifier}")
            .Append($"\t[{Methods.Count}] {nameof(Methods)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Methods))
            .Append($"\t[{Properties.Count}] {nameof(Properties)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Properties))
            .ToString();
    }
}