using CodeAnalyzer.Core.Models.SubModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Converters;

public class ReturnTypeConverter
{
    public ReturnType Convert(TypeSyntax typeSyntax)
    {
        return new ReturnType(typeSyntax.ToString());
    }
}