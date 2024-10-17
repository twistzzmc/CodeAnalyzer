namespace CodeAnalyzer.Providers;

public interface IFileProvider
{
    string Read(string path);
}