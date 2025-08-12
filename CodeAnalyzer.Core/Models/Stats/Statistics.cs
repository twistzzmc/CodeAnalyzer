using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Stats;

public sealed class Statistics
{
    private CboDto _cbo = CboDto.Empty;
    private WmpcDto _wmpc = WmpcDto.Empty;
    private CaDto _ca = CaDto.Empty;
    private AtfdDto _atfd = AtfdDto.Empty;
    private TccDto _tcc = TccDto.Empty;
    
    private bool _isCboSet;
    private bool _isWmpcSet;
    private bool _isCaSet;
    private bool _isAtfdSet;
    private bool _isTccSet;

    public CboDto Cbo { get => _cbo; set => SetStat(value, out _cbo, out _isCboSet); }
    public WmpcDto Wmpc { get => _wmpc; set => SetStat(value, out _wmpc, out _isWmpcSet);}
    public CaDto Ca { get => _ca; set => SetStat(value, out _ca, out _isCaSet); }
    public AtfdDto Atfd { get => _atfd; set => SetStat(value, out _atfd, out _isAtfdSet); }
    public TccDto Tcc { get => _tcc; set => SetStat(value, out _tcc, out _isTccSet); }
    
    public bool IsCboSet => _isCboSet;
    public bool IsWmpcSet => _isWmpcSet;
    public bool IsCaSet => _isCaSet;
    public bool IsAtfdSet => _isAtfdSet;
    public bool IsTccSet => _isTccSet;

    private static void SetStat<T>(T statValue, out T statField, out bool isStatSet)
    {
        statField = statValue;
        isStatSet = true;
    }
}