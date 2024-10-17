using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Data;

public sealed class WarningEventArgs(WarningType warningType, string message)
{
    public string? Id { get; private set; }
    public ModelType? ModelType { get; private set; }
    public WarningType WarningType { get; } = warningType;
    public string Message { get; } = message;

    public void ProvideContext(string id, ModelType modelType)
    {
        if (Id is not null && ModelType is not null)
        {
            throw new InvalidOperationException("Cannot set object data when the object is already set");
        }

        Id = id;
        ModelType = modelType;
    }
    
    public WarningData ToData()
    {
        if (Id is null)
        {
            throw new InvalidOperationException("Warning Model Id was not set");
        }

        if (ModelType is null)
        {
            throw new InvalidOperationException("Warning Model Type was not set");
        }

        return new WarningData(Id, ModelType.Value, WarningType, Message);
    }
}