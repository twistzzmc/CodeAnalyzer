using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Interfaces;

public interface IWarningRegistry
{
    event EventHandler<WarningData>? OnWarning; 
    event EventHandler<WarningEventArgs>? OnWarningNeedsContext;
    
    List<WarningData> Warnings { get; }
    
    void RegisterWarning(WarningType type, string message);

    void RegisterWarning(string id, ModelType modelType, WarningType type, string message);
}