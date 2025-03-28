namespace CodeAnalyzer.UI.Interfaces;

public interface ICodePathProvider
{
    string ProvideFile();

    string ProvideFolder();
}