using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;

namespace CodeAnalyzer.Core.Warnings;

public sealed class WarningRegistry : IWarningRegistry
{
    public event EventHandler<WarningData>? OnWarning;
    
    public List<WarningData> Warnings { get; } = [];
    public IdentifierDto? CurrentIdentifier { get; private set; }
    public ModelType CurrentModelType { get; private set; } = ModelType.Unknown;

    public void RegisterWarning(WarningType type, string message)
    {
        WarningData warningData = new(CurrentIdentifier, CurrentModelType, type, message);
        OnWarning?.Invoke(this, warningData);
        Warnings.Add(warningData);
    }

    public void SetContext(IdentifierDto identifier, ModelType modelType)
    {
        if (modelType == ModelType.Unknown)
        {
            throw new ArgumentException("Kontekst musi posiadać typ modelu", nameof(modelType));
        }
        
        CurrentIdentifier = identifier;
        CurrentModelType = modelType;
    }

    public void SetSimpleContext(string name, ModelType modelType)
    {
        SetContext(new IdentifierDto(string.Empty, name, []), modelType);
    }

    public void ClearContext()
    {
        CurrentIdentifier = null;
        CurrentModelType = ModelType.Unknown;
    }
}