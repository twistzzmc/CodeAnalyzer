using System.Collections.Concurrent;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Data;
using CodeAnalyzer.Core.Logging.Enums;
using CodeAnalyzer.Core.Logging.Interfaces;

namespace CodeAnalyzer.Core.Logging;

public sealed class WarningRegistry : IWarningRegistry
{
    public event EventHandler<WarningData>? OnWarning;

    private readonly ConcurrentQueue<WarningData> _warnings = new();

    private static readonly AsyncLocal<IdentifierDto?> CurrentIdentifierAsync = new();
    private static readonly AsyncLocal<ModelType> CurrentModelTypeAsync = new();

    public IReadOnlyCollection<WarningData> Warnings => _warnings.ToArray();
    public IdentifierDto? CurrentIdentifier => CurrentIdentifierAsync.Value;
    public ModelType CurrentModelType => CurrentModelTypeAsync.Value;

    public void RegisterWarning(WarningType type, string message)
    {
        IdentifierDto? identifier = CurrentIdentifier;
        ModelType modelType = CurrentModelType;

        WarningData warningData = new(identifier, modelType, type, message);

        EventHandler<WarningData>? handler = OnWarning;
        handler?.Invoke(this, warningData);

        _warnings.Enqueue(warningData);
    }

    public void SetContext(IdentifierDto identifier, ModelType modelType)
    {
        if (modelType == ModelType.Unknown)
        {
            throw new ArgumentException("Kontekst musi posiadać typ modelu", nameof(modelType));
        }

        CurrentIdentifierAsync.Value = identifier;
        CurrentModelTypeAsync.Value = modelType;
    }

    public void SetSimpleContext(string name, ModelType modelType)
    {
        SetContext(new IdentifierDto(string.Empty, name, []), modelType);
    }

    public void ClearContext()
    {
        CurrentIdentifierAsync.Value = null;
        CurrentModelTypeAsync.Value = ModelType.Unknown;
    }
}
