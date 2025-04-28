using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators.Factories;

internal sealed class CalculatorFactory(IWarningRegistry warningRegistry, CSharpCompilation compilation)
    : ICalculatorFactory
{
    public ICalculator<AccessModifierType, SyntaxTokenList> CreateAccessModifierCalculator()
    {
        return new AccessModifierCalculator(warningRegistry);
    }

    public ICalculator<int, CSharpSyntaxNode> CreateCyclomaticComplexityCalculator()
    {
        return new CyclomaticComplexityCalculator(warningRegistry, compilation);
    }

    public ICalculator<IPropertyValue<int>, PropertyDeclarationSyntax> CreatePropertyCyclomaticComplexityCalculator()
    {
        return new CyclomaticComplexityCalculator(warningRegistry, compilation);
    }

    public ICalculator<int, CSharpSyntaxNode> CreateLengthCalculator()
    {
        return new LengthCalculator();
    }

    public ICalculator<IPropertyValue<int>, PropertyDeclarationSyntax> CreatePropertyLengthCalculator()
    {
        return new LengthCalculator();
    }

    public ICalculator<int, CSharpSyntaxNode> CreateLineCalculator()
    {
        return new LineCalculator();
    }

    ICalculator<IEnumerable<ReferenceInstance>, MethodDeclarationSyntax> ICalculatorFactory.CreateReferencesCalculator()
    {
        return new ReferencesCalculator(warningRegistry, compilation);
    }

    public ICalculator<PropertyReferences, PropertyDeclarationSyntax> CreatePropertyReferencesCalculator()
    {
        return new ReferencesCalculator(warningRegistry, compilation);
    }

    public ICalculator<ReturnType, TypeSyntax> CreateReturnTypeCalculator()
    {
        return new ReturnTypeCalculator();
    }
}