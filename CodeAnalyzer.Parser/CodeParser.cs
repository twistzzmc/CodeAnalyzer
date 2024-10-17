using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser;

public static class CodeParser
{
    public static ClassModel Parse(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        
        CodeWalker walker = new();
        walker.Visit(root);

        return walker.GetClassModel();
    }
}