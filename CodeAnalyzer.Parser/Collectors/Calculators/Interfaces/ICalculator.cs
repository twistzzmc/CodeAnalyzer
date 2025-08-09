namespace CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;

internal interface ICalculator<out TResult, in TOptions> 
{
    TResult Calculate(TOptions options);
}