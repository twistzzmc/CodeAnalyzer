
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser;

const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Logger.cs";

string code = File.ReadAllText(classFilePath);
ClassModel model = CodeParser.Parse(code);

Console.WriteLine(model.ToString());
