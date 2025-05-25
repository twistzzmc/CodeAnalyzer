namespace CodeAnalyzer.Analyzer.Interfaces;

public interface IAnalysisConfiguration<out TConfigurationParameters>
    where TConfigurationParameters : class
{
    TConfigurationParameters WarningThreshold { get; }
    
    TConfigurationParameters ProblemThreshold { get; }
}