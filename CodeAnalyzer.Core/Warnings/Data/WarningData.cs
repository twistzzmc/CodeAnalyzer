using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Warnings.Enums;

namespace CodeAnalyzer.Core.Warnings.Data;

public record WarningData(
    IdentifierDto? Identifier,
    ModelType ModelType,
    WarningType WarningType,
    string Message);