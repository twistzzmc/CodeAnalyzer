using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Models.SubModels.Extensions;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Calculators.Factories;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class PropertyCollector(IWarningRegistry warningRegistry, ICalculatorFactory calculatorFactory)
    : BaseCollector<PropertyModel, PropertyDeclarationSyntax>(warningRegistry)
{
    protected override ModelType CollectorType => ModelType.Property;

    public PropertyCollector(IWarningRegistry warningRegistry, CSharpCompilation compilation)
        : this(warningRegistry, new CalculatorFactory(warningRegistry, compilation))
    { }

    protected override PropertyModel InnerCollect(PropertyDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        
        return new PropertyModel
        {
            Identifier = CurrentModelIdentifier,
            AccessModifierType = calculatorFactory.CreateAccessModifierCalculator().Calculate(node.Modifiers),
            Type = calculatorFactory.CreateReturnTypeCalculator().Calculate(node.Type),
            LineStart = calculatorFactory.CreateLineCalculator().Calculate(node),
            Length = calculatorFactory.CreatePropertyLengthCalculator().Calculate(node).ToLengthDto(),
            CyclomaticComplexity = calculatorFactory.CreatePropertyCyclomaticComplexityCalculator().Calculate(node).ToComplexityDto(),
            References = calculatorFactory.CreatePropertyReferencesCalculator().Calculate(node).ToReferenceDto()
        };
    }

    protected override string GetName(PropertyDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }
}