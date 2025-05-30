namespace CodeAnalyzer.Core.Models.Stats;

public sealed class Statistics
{
    private int _cbo;
    private int _wmpc;
    private FanInDto _fanIn = FanInDto.Empty;
    private int _atfd;
    private double _tcc;
    
    private bool _isCboSet;
    private bool _isWmpcSet;
    private bool _isFanInSet;
    private bool _isAtfdSet;
    private bool _isTccSet;

    public int Cbo { get => _cbo; set => SetStat(value, out _cbo, out _isCboSet); }
    public int Wmpc { get => _wmpc; set => SetStat(value, out _wmpc, out _isWmpcSet);}
    public FanInDto FanIn { get => _fanIn; set => SetStat(value, out _fanIn, out _isFanInSet); }
    public int Atfd { get => _atfd; set => SetStat(value, out _atfd, out _isAtfdSet); }
    public double Tcc { get => _tcc; set => SetStat(value, out _tcc, out _isTccSet); }
    
    public bool IsCboSet => _isCboSet;
    public bool IsWmpcSet => _isWmpcSet;
    public bool IsFanInSet => _isFanInSet;
    public bool IsAtfdSet => _isAtfdSet;
    public bool IsTccSet => _isTccSet;

    private static void SetStat<T>(T statValue, out T statField, out bool isStatSet)
    {
        statField = statValue;
        isStatSet = true;
    }
}