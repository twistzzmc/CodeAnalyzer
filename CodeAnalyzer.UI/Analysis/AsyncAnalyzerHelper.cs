using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser;
using CodeAnalyzer.UI.Analysis.Builders;
using CodeAnalyzer.UI.Analysis.Loggers;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class AsyncAnalyzerHelper
{
    private readonly ClassEntryBuilder _classEntryBuilder = new();
    private readonly AnalysisResultLogBuilder _analysisResultLogBuilder = new();
    private readonly List<ClassModelResult> _results = [];
    private readonly LogEntry _classEntry = new("Liczba znalezionych klas: 0");

    private bool _isTopClassEntryAdded = false;
    
    public async Task ParseCode(IWarningRegistry warningRegistry, ILogger logger, IEnumerable<string> code)
    {   
        await Task.Run(() =>
        {
            _classEntry.ClearChildren();
            _classEntry.Title = "Liczba znalezionych klas: 0";
            foreach (ClassModel result in CodeParser.Parse(warningRegistry, logger, code))
            {
                _results.Add(new ClassModelResult(result));
            }
        });
    }

    public async Task LogModels(ILoggerUi logger)
    {
        await Task.Run(() =>
        {
            _classEntry.Title = $"Liczba znalezionych klas: {_results.Count}";

            if (!_isTopClassEntryAdded)
            {
                logger.AddEntry(_classEntry);
            }
        
            foreach (ClassModelResult model in _results)
            {
                model.AddModelEntry(_classEntryBuilder);
                _classEntry.AddChild(model.ModelEntry);
            }
        });
    }

    public async Task RunMtlAnalysis(ILoggerUi logger)
    {
        await Task.Run(() =>
        {
            MtlAnalyzer mtlAnalyzer = new();
            List<MtlResultDto> mtlAnalysisResults = [];
            foreach (ClassModelResult model in _results)
            {
                mtlAnalysisResults.AddRange(mtlAnalyzer.Analyze(model.Model.Methods));
            }
            
            MtlLogger.Log(logger, mtlAnalysisResults);
        });
    }

    public async Task RunGodObjectAnalysis(ILoggerUi logger)
    {
        await Task.Run(() =>
        {
            GodObjectAnalyzer godObjectAnalyzer = new(_results.Select(r => r.Model));
            foreach (ClassModelResult model in _results)
            {
                model.AddGodObjectAnalysis(godObjectAnalyzer, _analysisResultLogBuilder);
            }

            GodObjectLogger godObjectLogger = new(logger);
            godObjectLogger.Log(_results.Select(r => r.GodObjectResult));
        });
    }
}