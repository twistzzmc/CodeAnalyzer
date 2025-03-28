using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Interfaces;

public interface IWarningRegistry
{
    List<WarningData> Warnings { get; }
    event EventHandler<WarningData>? OnWarning;
    event EventHandler<WarningEventArgs>? OnWarningNeedsContext;

    void RegisterWarning(WarningType type, string message);

    void RegisterWarning(IdentifierDto identifier, ModelType modelType, WarningType type, string message);
}