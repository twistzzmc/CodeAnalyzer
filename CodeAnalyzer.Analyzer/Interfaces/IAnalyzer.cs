using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Analyzer.Interfaces;

internal interface IAnalyzer<in TModel, out TResult>
    where TModel : IModel
    where TResult : IAnalysisResult<TModel>
{
    TResult Analyze(TModel model);
    
    IEnumerable<TResult> Analyze(IEnumerable<TModel> models);
}