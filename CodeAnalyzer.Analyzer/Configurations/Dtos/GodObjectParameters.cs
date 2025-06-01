namespace CodeAnalyzer.Analyzer.Configurations.Dtos;

public sealed class GodObjectParameters
{
    private int _wmpc;
    private int _cbo;
    private int _atfd;
    private double _tcc;
    private double _fanInMedian;

    public int Wmpc
    {
        get => _wmpc;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }

            _wmpc = value;
        }
    }

    public int Cbo
    {
        get => _cbo;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }
            
            _cbo = value;
        }
    }

    public int Atfd
    {
        get => _atfd;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }
            
            _atfd = value;
        }
    }

    public double Tcc
    {
        get => _tcc;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }
            
            _tcc = value;
        }
    }

    public double FanInMedian
    {
        get => _fanInMedian;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }
            
            _fanInMedian = value;
        }
    }
    
    internal static GodObjectParameters DefaultWarning => new(50, 10, 5, 0.3, 3);
    internal static GodObjectParameters DefaultProblem => new(75, 14, 10, 0.15, 4);

    public GodObjectParameters(int wmpc, int cbo, int atfd, double tcc, double fanInMedian)
    {
        Wmpc = wmpc;
        Cbo = cbo;
        Atfd = atfd;
        Tcc = tcc;
        FanInMedian = fanInMedian;
    }
}