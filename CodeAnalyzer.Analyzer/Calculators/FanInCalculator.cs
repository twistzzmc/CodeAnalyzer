using CodeAnalyzer.Analyzer.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Stats;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Analyzer.Calculators;

internal sealed class FanInCalculator(IEnumerable<ClassModel> allClasses) : IClassStatCalculator
{
    public void Calculate(ClassModel model)
    {
        bool containsSelfInAllClasses = false;
        int classesWithReferencesCount = 0;
        List<ClassModel> referenceClasses = [];

        foreach (ClassModel otherClass in allClasses)
        {
            if (otherClass.Identifier == model.Identifier)
            {
                containsSelfInAllClasses = true;
                continue;
            }

            if (HasReference(model, otherClass))
            {
                classesWithReferencesCount++;
                referenceClasses.Add(otherClass);
            }
        }

        double fanInPercentage = CalculatePercentage(containsSelfInAllClasses, classesWithReferencesCount);
        
        model.Stats.FanIn = new FanInDto()
        {
            FanIn = classesWithReferencesCount,
            FanInPercentage = fanInPercentage,
            ReferencesClassModels = referenceClasses
        };
    }

    private double CalculatePercentage(bool containsSelfInAllClasses, int classesWithReferencesCount)
    {
        int allClassesCount = containsSelfInAllClasses ? allClasses.Count() - 1 : allClasses.Count();
        return (double)classesWithReferencesCount / allClassesCount * 100;
    }

    private static bool HasReference(ClassModel model, ClassModel otherClass)
    {
        return otherClass.Methods.SelectMany(m => m.References)
            .Concat(otherClass.Properties.SelectMany(p => p.References))
            .Concat(otherClass.Fields.SelectMany(f => f.References))
            .Any(r => r.Namespace == model.Identifier.FullName);
    }
}