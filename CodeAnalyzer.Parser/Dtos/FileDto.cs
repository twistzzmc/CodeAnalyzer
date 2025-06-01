namespace CodeAnalyzer.Parser.Dtos;

public sealed class FileDto(string path, string data)
{
    public string Path { get; } = path;
    public string Data { get; } = data;

    public string Name => System.IO.Path.GetFileName(Path);
    public int LineCount => Data.Count(c => c == '\n') + 1;
    public int CharCount => Data.Length;
}