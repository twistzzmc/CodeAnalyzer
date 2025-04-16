namespace CodeAnalyzer.Parser.Collectors.Interfaces;

internal interface ICalculator<out TResult, in TOptions> 
{
    TResult Calculate(TOptions options);
}