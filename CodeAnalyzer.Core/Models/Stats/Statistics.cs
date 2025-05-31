using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Stats;

public sealed class Statistics
{
    private CboDto _cbo = CboDto.Empty;
    private int _wmpc;
    private FanInDto _fanIn = FanInDto.Empty;
    private AtfdData _atfd = AtfdData.Empty;
    private TccDto _tcc = TccDto.Empty;
    
    private bool _isCboSet;
    private bool _isWmpcSet;
    private bool _isFanInSet;
    private bool _isAtfdSet;
    private bool _isTccSet;

    public CboDto Cbo { get => _cbo; set => SetStat(value, out _cbo, out _isCboSet); }
    public int Wmpc { get => _wmpc; set => SetStat(value, out _wmpc, out _isWmpcSet);}
    public FanInDto FanIn { get => _fanIn; set => SetStat(value, out _fanIn, out _isFanInSet); }
    public AtfdData Atfd { get => _atfd; set => SetStat(value, out _atfd, out _isAtfdSet); }
    public TccDto Tcc { get => _tcc; set => SetStat(value, out _tcc, out _isTccSet); }
    
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