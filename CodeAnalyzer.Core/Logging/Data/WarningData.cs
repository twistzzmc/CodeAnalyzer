using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Enums;

namespace CodeAnalyzer.Core.Logging.Data;

public record WarningData(
    IdentifierDto? Identifier,
    ModelType ModelType,
    WarningType WarningType,
    string Message);