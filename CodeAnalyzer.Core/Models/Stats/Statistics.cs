using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Stats;

public sealed class Statistics
{
    private CboDto _cbo = CboDto.Empty;
    private WmcDto _wmc = WmcDto.Empty;
    private CaDto _ca = CaDto.Empty;
    private AtfdDto _atfd = AtfdDto.Empty;
    private TccDto _tcc = TccDto.Empty;
    
    private bool _isCboSet;
    private bool _isWmcSet;
    private bool _isCaSet;
    private bool _isAtfdSet;
    private bool _isTccSet;

    public CboDto Cbo { get => _cbo; set => SetStat(value, out _cbo, out _isCboSet); }
    public WmcDto Wmc { get => _wmc; set => SetStat(value, out _wmc, out _isWmcSet);}
    public CaDto Ca { get => _ca; set => SetStat(value, out _ca, out _isCaSet); }
    public AtfdDto Atfd { get => _atfd; set => SetStat(value, out _atfd, out _isAtfdSet); }
    public TccDto Tcc { get => _tcc; set => SetStat(value, out _tcc, out _isTccSet); }
    
    public bool IsCboSet => _isCboSet;
    public bool IsWmcSet => _isWmcSet;
    public bool IsCaSet => _isCaSet;
    public bool IsAtfdSet => _isAtfdSet;
    public bool IsTccSet => _isTccSet;

    private static void SetStat<T>(T statValue, out T statField, out bool isStatSet)
    {
        statField = statValue;
        isStatSet = true;
    }
}