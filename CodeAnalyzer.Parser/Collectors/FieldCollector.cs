using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Calculators.Factories;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class FieldCollector(
    IWarningRegistry warningRegistry,
    ICalculatorFactory calculatorFactory,
    FieldDeclarationSyntax field)
    : BaseCollector<FieldModel, VariableDeclaratorSyntax>(warningRegistry)
{
    protected override ModelType CollectorType => ModelType.Field;
    
    public FieldCollector(IWarningRegistry warningRegistry, CSharpCompilation compilation, FieldDeclarationSyntax field)
        : this(warningRegistry, new CalculatorFactory(warningRegistry, compilation), field)
    { }
    
    protected override FieldModel InnerCollect(VariableDeclaratorSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));

        return new FieldModel
        {
            Identifier = CurrentModelIdentifier,
            AccessModifierType = calculatorFactory.CreateAccessModifierCalculator().Calculate(field.Modifiers),
            Type = calculatorFactory.CreateReturnTypeCalculator().Calculate(field.Declaration.Type),
            LineStart = calculatorFactory.CreateLineCalculator().Calculate(node),
            References = calculatorFactory.CreateVariableReferencesCalculator().Calculate(node).ToList()
        };
    }

    protected override string GetName(VariableDeclaratorSyntax node)
    {
        return node.Identifier.Text;
    }
}