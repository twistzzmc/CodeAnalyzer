using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Collectors.Calculators;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using CodeAnalyzer.Parser.Guards;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector(
    IWarningRegistry warningRegistry,
    ICalculator<int, MemberDeclarationSyntax> cyclomaticComplexityCalculator,
    ICalculator<IEnumerable<ReferenceInstance>, MemberDeclarationSyntax> referencesCalculator,
    ICalculator<int, MemberDeclarationSyntax> lengthCalculator,
    ICalculator<int, MemberDeclarationSyntax> lineCalculator,
    ICalculator<AccessModifierType, SyntaxTokenList> accessModifierCalculator,
    ICalculator<ReturnType, TypeSyntax> returnTypeCalculator)
    : BaseCollector<MethodModel, MethodDeclarationSyntax>(warningRegistry)
{
    private readonly IWarningRegistry _warningRegistry = warningRegistry;
    private readonly ICalculator<int, MemberDeclarationSyntax> _lengthCalculator = lengthCalculator;

    protected override ModelType CollectorType => ModelType.Method;

    public MethodCollector(IWarningRegistry warningRegistry, CSharpCompilation compilation)
        : this(
            warningRegistry,
            new CyclomaticComplexityCalculator(warningRegistry, compilation),
            new ReferencesCreator(warningRegistry, compilation),
            new LengthCalculator(),
            new LineCalculator(),
            new AccessModifierCalculator(warningRegistry),
            new ReturnTypeCalculator())
    { }
    
    protected override MethodModel InnerCollect(MethodDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        ModifierGuard.GuardAgainstUnknown(_warningRegistry, node.Modifiers.Select(m => m.Text));

        AccessModifierType modifier = accessModifierCalculator.Calculate(node.Modifiers);
        ReturnType returnType = returnTypeCalculator.Calculate(node.ReturnType);
        int startLine = lineCalculator.Calculate(node);
        int length = _lengthCalculator.Calculate(node);
        int cyclomaticComplexity = cyclomaticComplexityCalculator.Calculate(node);
        List<ReferenceInstance> references = referencesCalculator.Calculate(node).ToList();

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