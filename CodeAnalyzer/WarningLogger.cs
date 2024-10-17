using CodeAnalyzer.Core.Warnings.Data;

namespace CodeAnalyzer;

public static class WarningLogger
{
    public static void Log(object? sender, WarningData warningData)
    {
        Console.WriteLine(warningData.ToString());
    }
}