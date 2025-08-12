namespace CodeAnalyzer.Analyzer.Configurations.Dtos;

public sealed class GodObjectParameters
{
    private int _wmpc;
    private int _cbo;
    private int _atfd;
    private double _tcc;
    private double _ca;

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

    public double Ca
    {
        get => _ca;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Wartość musi być większa od 0");
            }
            
            _ca = value;
        }
    }
    
    internal static GodObjectParameters DefaultWarning => new(35, 14, 3, 0.33, 10);
    internal static GodObjectParameters DefaultProblem => new(47, 14, 5, 0.5, 10);

    public GodObjectParameters(int wmpc, int cbo, int atfd, double tcc, double ca)
    {
        Wmpc = wmpc;
        Cbo = cbo;
        Atfd = atfd;
        Tcc = tcc;
        Ca = ca;
    }
}