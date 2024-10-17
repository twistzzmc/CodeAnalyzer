
using CodeAnalyzer;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings;
using CodeAnalyzer.Parser;

const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Logger.cs";

WarningRegistry registry = new();
registry.OnWarning += WarningLogger.Log;

string code = File.ReadAllText(classFilePath);
ClassModel model = CodeParser.Parse(registry, code);

Console.WriteLine(model.ToString());
