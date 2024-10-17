// See https://aka.ms/new-console-template for more information

using CodeAnalyzer.Models;
using CodeAnalyzer.Providers;
using CodeAnalyzer.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

string loggerPath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Logger.cs";

CSharpFileProvider fileProvider = new();
string code = fileProvider.Read(loggerPath);

SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();


CodeWalker walker = new();
walker.Visit(root);

ClassModel classModel = walker.GetClassModel();

Console.WriteLine($"ClassModel: {classModel}");
