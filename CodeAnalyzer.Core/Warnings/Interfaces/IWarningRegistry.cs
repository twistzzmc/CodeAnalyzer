using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Data;
using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Interfaces;

public interface IWarningRegistry
{
    event EventHandler<WarningData>? OnWarning;
    
    List<WarningData> Warnings { get; }
    IdentifierDto? CurrentIdentifier { get; }
    ModelType CurrentModelType { get; }

    void RegisterWarning(WarningType type, string message);
    void SetContext(IdentifierDto identifier, ModelType modelType);
    void SetSimpleContext(string name, ModelType modelType);
    void ClearContext();
}