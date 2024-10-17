using System.Text;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class ClassModel(string name, IEnumerable<MethodModel> methods, IEnumerable<PropertyModel> properties) : IModel
{
    private static readonly string ListDelimeter = $"{Environment.NewLine}\t\t";
    
    public string Name => name;
    public IReadOnlyList<MethodModel> Methods => methods.ToList();
    public IReadOnlyList<PropertyModel> Properties => properties.ToList();

    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"{nameof(ClassModel)}(")
            .AppendLine($"\t{nameof(Name)}: {Name}")
            .Append($"\t[{Methods.Count}] {nameof(Methods)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Methods))
            .Append($"\t[{Properties.Count}] {nameof(Properties)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Properties))
            .ToString();
    }
}