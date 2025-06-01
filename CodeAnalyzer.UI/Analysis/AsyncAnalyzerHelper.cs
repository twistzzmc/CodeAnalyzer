using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser;
using CodeAnalyzer.Parser.Dtos;
using CodeAnalyzer.UI.LoggerUi.Builders.AnalysisResultBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class AsyncAnalyzerHelper
{
    private readonly GodObjectResultLogBuilder _godObjectResultLogBuilder = new();
    private readonly MtlResultLogBuilder _mtlResultLogBuilder = new();
    private readonly ClassEntryBuilder _classEntryBuilder = new();
    private readonly List<ClassModelResult> _results = [];
    private readonly LogEntry _classEntry = new("Liczba znalezionych klas: 0");

    private bool _isTopClassEntryAdded = false;
    
    public async Task ParseCode(IWarningRegistry warningRegistry, ILogger logger, IEnumerable<FileDto> code)
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
            
            logger.AddEntry(_mtlResultLogBuilder.Build(mtlAnalysisResults));
        });
    }

    public async Task RunGodObjectAnalysis(ILogger logger, ILoggerUi resultLogger)
    {
        await Task.Run(() =>
        {
            try
            {
                logger.OpenLevel("Analiza obiektów Bóg");
                GodObjectAnalyzer godObjectAnalyzer = new(_results.Select(r => r.Model));
                foreach (ClassModelResult model in _results)
                {
                    logger.Info($"Klasa {model.Model.Identifier.FullName}");
                    
                    try
                    {
                        model.AddGodObjectAnalysis(godObjectAnalyzer);
                    }
                    catch (Exception ex)
                    {
                        logger.Exception(ex);
                    }
                }

                resultLogger.AddEntry(_godObjectResultLogBuilder.Build(_results.Select(r => r.GodObjectResult)));
            }
            finally
            {
                logger.CloseLevel();
            }
        });
    }
}