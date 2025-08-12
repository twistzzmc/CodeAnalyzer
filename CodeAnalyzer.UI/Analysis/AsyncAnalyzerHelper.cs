using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Analyzer;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Parser;
using CodeAnalyzer.Parser.Dtos;
using CodeAnalyzer.UI.LoggerUi.Builders.AnalysisResultBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Builders.SubModelEntryBuilders;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class AsyncAnalyzerHelper
{
    private readonly GodObjectResultLogBuilder _godObjectResultLogBuilder = new();
    private readonly PercentileThresholdsBuilder _percentileThresholdsBuilder = new();
    private readonly MtcResultLogBuilder _mtcResultLogBuilder = new();
    private readonly ClassEntryBuilder _classEntryBuilder = new();
    private readonly List<ClassModelResult> _results = [];
    private readonly LogEntry _classEntry = new("Liczba znalezionych klas: 0");

    private bool _isTopClassEntryAdded = false;
    
    public async Task ParseCode(IWarningRegistry warningRegistry, ILogger logger, IEnumerable<FileDto> code)
    {   
        
        _classEntry.ClearChildren();
        _classEntry.Title = "Liczba znalezionych klas: 0";
        IEnumerable<ClassModel> models = await CodeParser.ParseAsync(warningRegistry, logger, code);
        foreach (ClassModel result in models)
        {
            _results.Add(new ClassModelResult(result));
        }
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

    public async Task RunMtcAnalysis(ILoggerUi logger)
    {
        await Task.Run(() =>
        {
            MtcAnalyzer mtcAnalyzer = new();
            List<MtcResultDto> mtcAnalysisResults = [];
            foreach (ClassModelResult model in _results)
            {
                mtcAnalysisResults.AddRange(mtcAnalyzer.Analyze(model.Model.Methods));
            }
            
            logger.AddEntry(_mtcResultLogBuilder.Build(mtcAnalysisResults));
        });
    }

    public async Task RunGodObjectAnalysis(ILogger logger, ILoggerUi resultLogger)
    {
        await Task.Run(() =>
        {
            try
            {
                logger.OpenLevel("Analiza obiektów Bóg");
                List<ClassModel> classModels = _results.Select(r => r.Model).ToList();
                GodObjectAnalyzer godObjectAnalyzer = new(classModels);
                
                godObjectAnalyzer.PreAnalysis();
                resultLogger.AddEntry(_percentileThresholdsBuilder.Build(godObjectAnalyzer.Thresholds));
                
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

                IEnumerable<GodObjectResultDto?> analysisResults = _results.Select(r => r.GodObjectResult);
                resultLogger.AddEntry(_godObjectResultLogBuilder.Build(analysisResults));
            }
            finally
            {
                logger.CloseLevel();
            }
        });
    }
}