namespace CodeAnalyzer.Core.Models.Stats;

public sealed class Statistics
{
    private int _cbo;
    private int _wmpc;
    private FanInDto _fanIn = FanInDto.Empty;
    
    private bool _isCboSet;
    private bool _isWmpcSet;
    private bool _isFanInSet;

    public int Cbo { get => _cbo; set => SetStat(value, out _cbo, out _isCboSet); }
    public int Wmpc { get => _wmpc; set => SetStat(value, out _wmpc, out _isWmpcSet);}
    public FanInDto FanIn { get => _fanIn; set => SetStat(value, out _fanIn, out _isFanInSet); }
    
    public bool IsCboSet => _isCboSet;
    public bool IsWmpcSet => _isWmpcSet;
    public bool IsFanInSet => _isFanInSet;

    private static void SetStat<T>(T statValue, out T statField, out bool isStatSet)
    {
        statField = statValue;
        isStatSet = true;
    }
}