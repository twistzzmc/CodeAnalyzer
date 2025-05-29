using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Analyzer.Interfaces;

internal interface IClassStatCalculator
{
    void Calculate(ClassModel model);
}