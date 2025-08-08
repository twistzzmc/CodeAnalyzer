using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Results.GodObject;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Calculators.GodObject;

internal sealed class ConstantIssueCalculator
{
    public ConstantMetric Calculate(ClassModel model)
    {
        double score = CalculateGodObjectScore(
            model.Stats.Wmpc.Wmpc,
            model.Stats.Atfd.Atfd,
            model.Stats.Tcc.Tcc,
            model.Stats.Cbo.Cbo,
            model.Stats.FanIn.FanIn);
        
        IssueCertainty issueCertainty = score >= 80
            ? IssueCertainty.Problem
            : score >= 60 ? IssueCertainty.Warning : IssueCertainty.Info;
    
        bool isMarinescu = IsMarinescu(model.Stats.Wmpc.Wmpc, model.Stats.Atfd.Atfd, model.Stats.Tcc.Tcc);
        if (!isMarinescu)
        {
            issueCertainty = IssueCertainty.Info;
        }

        return new ConstantMetric
        {
            CertaintyPercent = score,
            Marinescu = isMarinescu,
            Certainty = issueCertainty
        };
    }
    
    private static bool IsMarinescu(int wmpc, int atfd, double tcc)
    {
        return wmpc >= 47 && atfd > 5 && tcc < 0.33;
    }
    
    private static double CalculateGodObjectScore(int wmc, int atfd, double tcc, int cbo, int ca)
    {
        double normWmc  = Math.Min(1.0, wmc / 60.0);
        double normAtfd = Math.Min(1.0, atfd / 10.0);
        double normTcc  = 1.0 - Math.Min(1.0, tcc / 0.33);
        double normCbo  = Math.Min(1.0, cbo / 20.0);
        double normCa   = Math.Min(1.0, ca / 15.0);

        double score = 
            0.25 * normWmc +
            0.25 * normAtfd +
            0.20 * normTcc +
            0.15 * normCbo +
            0.15 * normCa;

        return score * 100.0;
    }
}