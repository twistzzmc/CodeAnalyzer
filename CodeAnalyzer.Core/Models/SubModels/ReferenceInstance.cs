using CodeAnalyzer.Core.Identifiers;

namespace CodeAnalyzer.Core.Models.SubModels;

public sealed class ReferenceInstance
{
    public required string Namespace { get; init; }
    public required int LineNumber { get; init; }
    public required int ColumnNumber { get; init; }
}