﻿using CodeAnalyzer.Core.Models;
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
        return Walk(Compile(codes));
    }

    public IEnumerable<ClassModel> Parse(string code)
    {
        return Walk(Compile([code]));
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, string code)
    {
        return new CodeParser(warningRegistry).Parse(code);
    }

    public static IEnumerable<ClassModel> Parse(IWarningRegistry warningRegistry, IEnumerable<string> codes)
    {
        return new CodeParser(warningRegistry).Parse(codes);
    }

    private IEnumerable<ClassModel> Walk(CSharpCompilation compilation)
    {
        CodeWalker walker = new(warningRegistry, compilation);

        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            walker.Visit(tree.GetRoot());
        }
        
        return walker.CollectClassModels();
    }

    private CSharpCompilation Compile(IEnumerable<string> codes)
    {
        List<SyntaxTree> syntaxTrees = codes.Select(code => CSharpSyntaxTree.ParseText(code)).ToList();

        IEnumerable<PortableExecutableReference> references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));
        
        return CSharpCompilation.Create("FullAnalysis")
            .AddSyntaxTrees(syntaxTrees)
            .AddReferences(references);
    }
}