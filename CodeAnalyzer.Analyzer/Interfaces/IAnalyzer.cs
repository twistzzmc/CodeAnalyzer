using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Analyzer.Interfaces;

// internal interface IAnalyzer<out TAnalysisParameters, in TModel, out TResult>
//     where TAnalysisParameters : class
//     where TModel : IModel
//     where TResult : IAnalysisResult<TModel>
// {
//     IAnalysisConfiguration<TAnalysisParameters> Configuration { get; }
//     
//     TResult Analyze(TModel model);
//     
//     IEnumerable<TResult> Analyze(IEnumerable<TModel> models);
// }

internal interface IAnalyzer<out TConfiguration, out TConfigurationParameters, in TModel, out TResult>
    where TConfiguration : IAnalysisConfiguration<TConfigurationParameters>
    where TConfigurationParameters : class
    where TModel : IModel
    where TResult : IAnalysisResult<TModel>
{
    TConfiguration Configuration { get; }
    
    TResult Analyze(TModel model);
    
    IEnumerable<TResult> Analyze(IEnumerable<TModel> models);
}