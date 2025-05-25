using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Data;
using CodeAnalyzer.Core.Logging.Enums;

namespace CodeAnalyzer.Core.Logging.Interfaces;

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