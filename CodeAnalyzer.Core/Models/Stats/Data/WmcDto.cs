namespace CodeAnalyzer.Core.Models.Stats.Data;

/// <summary>
/// Weighted Methods per Class
/// </summary>
public sealed class WmcDto(int wmc)
{
    public int Wmc { get; set; } = wmc;
    
    public static WmcDto Empty = new WmcDto(0);
}