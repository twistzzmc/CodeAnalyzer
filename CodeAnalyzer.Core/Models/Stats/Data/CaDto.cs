namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Afferent Coupling
/// </summary>
public sealed class CaDto
{
    public required int Ca { get; init; }
    public required double CaPercentage { get; init; }
    public required List<ClassModel> ReferencesClassModels { get; init; }

    public static CaDto Empty { get; } = new()
    {
        Ca = 0,
        CaPercentage = 0,
        ReferencesClassModels = []
    };
}