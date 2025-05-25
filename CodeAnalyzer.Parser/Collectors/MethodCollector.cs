using CodeAnalyzer.Core.Logging.Enums;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Parser.Collectors.Calculators;
using CodeAnalyzer.Parser.Collectors.Calculators.Factories;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using CodeAnalyzer.Parser.Collectors.Factories;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using CodeAnalyzer.Parser.Guards;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector( IWarningRegistry warningRegistry, ICalculatorFactory calculatorFactory)
    : BaseCollector<MethodModel, MethodDeclarationSyntax>(warningRegistry)
{
    private readonly IWarningRegistry _warningRegistry = warningRegistry;

    protected override ModelType CollectorType => ModelType.Method;

    public MethodCollector(IWarningRegistry warningRegistry, CSharpCompilation compilation)
        : this(warningRegistry, new CalculatorFactory(warningRegistry, compilation))
    { }
    
    protected override MethodModel InnerCollect(MethodDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        ModifierGuard.GuardAgainstUnknown(_warningRegistry, node.Modifiers.Select(m => m.Text));

        AccessModifierType modifier = calculatorFactory.CreateAccessModifierCalculator().Calculate(node.Modifiers);
        ReturnType returnType = calculatorFactory.CreateReturnTypeCalculator().Calculate(node.ReturnType);
        int startLine = calculatorFactory.CreateLineCalculator().Calculate(node);
        int length = calculatorFactory.CreateLengthCalculator().Calculate(node);
        int cyclomaticComplexity = calculatorFactory.CreateCyclomaticComplexityCalculator().Calculate(node);
        List<ReferenceInstance> references = calculatorFactory.CreateMethodReferencesCalculator().Calculate(node).ToList();

        return new MethodModel
        {
            Identifier = CurrentModelIdentifier,
            AccessModifierType = modifier,
            ReturnType = returnType,
            LineStart = startLine,
            Length = length,
            CyclomaticComplexity = cyclomaticComplexity,
            References = references
        };
    }

    protected override string GetName(MethodDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }
}