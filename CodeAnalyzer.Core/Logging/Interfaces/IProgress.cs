namespace CodeAnalyzer.Core.Logging.Interfaces;

public interface IProgress
{
    uint Total { get; }
    uint Current { get; set; }
}