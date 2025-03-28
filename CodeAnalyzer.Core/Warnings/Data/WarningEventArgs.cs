using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Data;

public sealed class WarningEventArgs(WarningType warningType, string message)
{
    public IdentifierDto? Identifier { get; private set; }
    public ModelType? ModelType { get; private set; }
    public WarningType WarningType { get; } = warningType;
    public string Message { get; } = message;

    public void ProvideContext(IdentifierDto identifier, ModelType modelType)
    {
        if (Identifier is not null && ModelType is not null)
        {
            throw new InvalidOperationException("Cannot set object data when the object is already set");
        }

        Identifier = identifier;
        ModelType = modelType;
    }
    
    public WarningData ToData()
    {
        if (Identifier is null)
        {
            throw new InvalidOperationException("Warning Model Id was not set");
        }

        if (ModelType is null)
        {
            throw new InvalidOperationException("Warning Model Type was not set");
        }

        return new WarningData(Identifier, ModelType.Value, WarningType, Message);
    }
}