namespace CodeAnalyzer.Core.Interfaces;

public interface IFillable<in TFilled> where TFilled : class
{
    void Fill(TFilled other);
}