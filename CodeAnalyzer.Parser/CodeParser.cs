using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser;

public class CodeParser(IWarningRegistry warningRegistry)
{
    public ClassModel Parse(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        
        CodeWalker walker = new(warningRegistry, tree);
        walker.Visit(root);

        return walker.GetClassModel();
    }

    public static ClassModel Parse(IWarningRegistry warningRegistry, string code)
    {
        return new CodeParser(warningRegistry).Parse(code);
    }
}