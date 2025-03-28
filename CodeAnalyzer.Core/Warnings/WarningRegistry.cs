using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;

namespace CodeAnalyzer.Core.Warnings;

public sealed class WarningRegistry : IWarningRegistry
{
    public event EventHandler<WarningData>? OnWarning;
    public event EventHandler<WarningEventArgs>? OnWarningNeedsContext;

    public List<WarningData> Warnings { get; } = [];

    public void RegisterWarning(WarningType type, string message)
    {
        WarningEventArgs warningEventArgs = new(type, message);
        OnWarningNeedsContext?.Invoke(this, warningEventArgs);

        if (warningEventArgs.Identifier is null)
            throw new InvalidOperationException("Warning object data was not filled");

        WarningData warningData = warningEventArgs.ToData();
        OnWarning?.Invoke(this, warningData);
        Warnings.Add(warningData);
    }

    public void RegisterWarning(IdentifierDto identifier, ModelType modelType, WarningType warningType, string message)
    {
        WarningData warningData = new(identifier, modelType, warningType, message);
        OnWarning?.Invoke(this, warningData);
        Warnings.Add(warningData);
    }
}