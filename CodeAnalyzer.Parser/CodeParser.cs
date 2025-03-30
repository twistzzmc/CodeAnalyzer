using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Builders;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser;

public class CodeParser(IWarningRegistry warningRegistry)
{
    public IEnumerable<ClassModel> Parse(IEnumerable<string> codes)
    {
        ClassModelsBuilder builder = new();

        foreach (string code in codes) Walk(code, builder);

        return builder.Build();
    }

    public IEnumerable<ClassModel> Parse(string code)
    {
        return Walk(code).Builder.Build();
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, string code)
    {
        return new CodeParser(warningRegistry).Parse(code);
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, IEnumerable<string> codes)
    {
        return new CodeParser(warningRegistry).Parse(codes);
    }

    private CodeWalker Walk(string code, ClassModelsBuilder? builder = null)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        CodeWalker walker = new(warningRegistry, tree);
        if (builder is not null) walker.Builder = builder;

        walker.Visit(root);
        return walker;
    }
}