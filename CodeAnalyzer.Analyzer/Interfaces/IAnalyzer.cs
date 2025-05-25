using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Analyzer.Interfaces;

internal interface IAnalyzer<out TAnalysisParameters, in TModel, out TResult>
    where TAnalysisParameters : class
    where TModel : IModel
    where TResult : IAnalysisResult<TModel>
{
    IAnalysisConfiguration<TAnalysisParameters> Configuration { get; }
    
    TResult Analyze(TModel model);
    
    IEnumerable<TResult> Analyze(IEnumerable<TModel> models);
}