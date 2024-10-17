using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Data;

public record WarningData(string Id, ModelType ModelType, WarningType WarningType, string Message);