using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.Analysis.Loggers;

internal sealed class GodObjectLogger(ILoggerUi logger)
{
    public void Log(IEnumerable<GodObjectResultDto?> results)
    {
        int problemCount = results.Count(r => r.Certainty == IssueCertainty.Problem);
        int warningCount = results.Count(r => r.Certainty == IssueCertainty.Warning);
        
        logger.AddEntry($"Znalezione GodObject: {problemCount + warningCount}");
    }
}