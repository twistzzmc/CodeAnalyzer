using System.Text;

namespace CodeAnalyzer.Core.Models.SubModels;

public class ReturnType(string name)
{
    public string Name => name;

    public override string ToString()
    {
        return new StringBuilder()
            .Append($"{nameof(ReturnType)}(")
            .Append($"{nameof(Name)}: {Name}")
            .Append(')')
            .ToString();
    }
}