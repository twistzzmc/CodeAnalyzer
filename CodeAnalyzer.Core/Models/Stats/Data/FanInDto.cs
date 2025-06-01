namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Afferent Coupling
/// </summary>
public sealed class FanInDto
{
    public required int FanIn { get; init; }
    public required double FanInPercentage { get; init; }
    public required List<ClassModel> ReferencesClassModels { get; init; }

    public static FanInDto Empty { get; } = new()
    {
        FanIn = 0,
        FanInPercentage = 0,
        ReferencesClassModels = []
    };
}