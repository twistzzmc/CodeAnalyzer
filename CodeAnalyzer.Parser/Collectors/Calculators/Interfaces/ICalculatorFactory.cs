using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;

internal interface ICalculatorFactory
{
    ICalculator<AccessModifierType, SyntaxTokenList> CreateAccessModifierCalculator();
    
    ICalculator<int, CSharpSyntaxNode> CreateCyclomaticComplexityCalculator();
    
    ICalculator<IPropertyValue<int>, PropertyDeclarationSyntax> CreatePropertyCyclomaticComplexityCalculator();
    
    ICalculator<int, CSharpSyntaxNode> CreateLengthCalculator();
    
    ICalculator<IPropertyValue<int>, PropertyDeclarationSyntax> CreatePropertyLengthCalculator();
    
    ICalculator<int, CSharpSyntaxNode> CreateLineCalculator();
    
    ICalculator<IEnumerable<ReferenceInstance>, CSharpSyntaxNode> CreateReferencesCalculator();
    
    ICalculator<IPropertyValue<IEnumerable<ReferenceInstance>>, PropertyDeclarationSyntax> CreatePropertyReferencesCalculator();
    
    ICalculator<ReturnType, TypeSyntax> CreateReturnTypeCalculator();
}