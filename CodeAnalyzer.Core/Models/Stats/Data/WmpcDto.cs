namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Weighted Methods per Class
/// </summary>
public sealed class WmpcDto(int wmpc)
{
    public int Wmpc { get; set; } = wmpc;
    
    public static WmpcDto Empty = new WmpcDto(0);
}