
using Avalonia;
using CodeAnalyzer;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings;
using CodeAnalyzer.Parser;
using CodeAnalyzer.UI;

// // const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Logger.cs";
// const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/tests/NLog.UnitTests/Targets/FileTargetTests.cs";
// // const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Targets/AsyncTaskTarget.cs";
// // const string classFilePath = "/home/michal-szczepaniak/mleko/code-examples/NLog/src/NLog/Common/InternalLogger.cs";
//
// WarningRegistry registry = new();
// registry.OnWarning += WarningLogger.Log;
//
// string code = File.ReadAllText(classFilePath);
// ClassModel model = CodeParser.Parse(registry, code);
//
// Console.WriteLine(model.ToString());
// model.PrintTooLongMethods();

namespace CodeAnalyzer;

public static class Program
{
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime([]);
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
