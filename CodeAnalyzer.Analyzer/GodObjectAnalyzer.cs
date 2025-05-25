using CodeAnalyzer.Analyzer.Configurations;
using CodeAnalyzer.Analyzer.Configurations.Dtos;
using CodeAnalyzer.Analyzer.Enums;
using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Analyzer.Results;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer;

public sealed class GodObjectAnalyzer(IEnumerable<ClassModel> allClassModels)
    : IAnalyzer<GodObjectConfiguration, GodObjectParameters, ClassModel, GodObjectResultDto>
{
    private GodObjectParameters Warning => Configuration.WarningThreshold;
    private GodObjectParameters Problem => Configuration.ProblemThreshold;
    
    public GodObjectConfiguration Configuration { get; } = new GodObjectConfiguration()
    {
        ProblemThreshold = new GodObjectParameters(20),
        WarningThreshold = new GodObjectParameters(10)
    };
    
    public GodObjectResultDto Analyze(ClassModel model)
    {
        bool containsSelfInAllClasses = false;
        int classesWithReferencesCount = 0;

        foreach (ClassModel otherClass in allClassModels)
        {
            if (otherClass.Identifier == model.Identifier)
            {
                containsSelfInAllClasses = true;
                continue;
            }

            if (HasReference(model, otherClass))
            {
                classesWithReferencesCount++;
            }
        }

        double percentage = CalculatePercentage(containsSelfInAllClasses, classesWithReferencesCount);
        IssueCertainty issueCertainty = percentage > Problem.PercentageOfUsage
            ? IssueCertainty.Problem
            : percentage > Warning.PercentageOfUsage
                ? IssueCertainty.Warning
                : IssueCertainty.Info;

        return new GodObjectResultDto(model, AnalysisIssueType.GodObject, issueCertainty, percentage);
    }

    public IEnumerable<GodObjectResultDto> Analyze(IEnumerable<ClassModel> models)
    {
        return models.Select(Analyze);
    }

    private double CalculatePercentage(bool containsSelfInAllClasses, int classesWithReferencesCount)
    {
        int allClassesCount = containsSelfInAllClasses ? allClassModels.Count() - 1 : allClassModels.Count();
        return (double)allClassesCount / classesWithReferencesCount * 100;
    }

    private static bool HasReference(ClassModel model, ClassModel otherClass)
    {
        return otherClass.Methods.SelectMany(m => m.References)
            .Concat(otherClass.Properties.SelectMany(p => p.References))
            .Concat(otherClass.Fields.SelectMany(f => f.References))
            .Any(r => r.Namespace == model.Identifier.FullName);
    }
}