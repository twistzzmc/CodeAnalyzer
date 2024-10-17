namespace CodeAnalyzer.Providers;

public sealed class CSharpFileProvider : IFileProvider
{
    public string Read(string path)
    {
        return File.ReadAllText(path);
    }
}