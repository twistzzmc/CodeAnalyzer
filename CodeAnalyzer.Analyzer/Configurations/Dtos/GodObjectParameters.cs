namespace CodeAnalyzer.Analyzer.Configurations.Dtos;

public sealed class GodObjectParameters
{
    private int _percentageOfUsage;

    public int PercentageOfUsage
    {
        get => _percentageOfUsage;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 100");
            }

            _percentageOfUsage = value;
        }
    }

    public GodObjectParameters(int percentageOfUsage)
    {
        PercentageOfUsage = percentageOfUsage;
    }
}