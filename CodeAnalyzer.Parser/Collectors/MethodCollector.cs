using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Converters;
using CodeAnalyzer.Parser.Guards;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector(IWarningRegistry warningRegistry, SyntaxTree tree)
    : BaseCollector<MethodModel, MethodDeclarationSyntax>(warningRegistry)
{
    private readonly AccessModifierConverter _accessModifierConverter = new(warningRegistry);
    private readonly ReturnTypeConverter _returnTypeConverter = new();

    protected override ModelType CollectorType => ModelType.Method;

    protected override MethodModel InnerCollect(MethodDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(CurrentModelIdentifier, nameof(CurrentModelIdentifier));
        ModifierGuard.GuardAgainstUnknown(warningRegistry, node.Modifiers.Select(m => m.Text));

        AccessModifierType modifier = _accessModifierConverter.Convert(node.Modifiers);
        ReturnType returnType = _returnTypeConverter.Convert(node.ReturnType);
        int startLine = node.GetLocation().GetLineSpan().StartLinePosition.Line;
        int length = CalculateMethodLength(node);
        int cyclomaticComplexity = CalculateCyclomaticComplexity(node);

        return new MethodModel(
            CurrentModelIdentifier,
            modifier,
            returnType,
            startLine,
            length,
            cyclomaticComplexity);
    }

    protected override string GetName(MethodDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }

    private static int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
    {
        int complexity = 1; // Startujemy od 1, bo każda metoda ma co najmniej jedną ścieżkę wykonania

        // Instrukcje sterujące zwiększające złożoność
        complexity += method.DescendantNodes().OfType<IfStatementSyntax>().Count();
        complexity += method.DescendantNodes().OfType<ForStatementSyntax>().Count();
        complexity += method.DescendantNodes().OfType<ForEachStatementSyntax>().Count();
        complexity += method.DescendantNodes().OfType<WhileStatementSyntax>().Count();
        complexity += method.DescendantNodes().OfType<DoStatementSyntax>().Count();
        complexity += method.DescendantNodes().OfType<CaseSwitchLabelSyntax>().Count(); // Każdy case w switch
        complexity += method.DescendantNodes().OfType<CatchClauseSyntax>().Count();
        complexity +=
            method.DescendantNodes().OfType<ConditionalExpressionSyntax>().Count(); // Warunek ternarny (x ? y : z)

        return complexity;
    }

    private static int CalculateMethodLength(MethodDeclarationSyntax node)
    {
        int startLine = node.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endLine = node.GetLocation().GetLineSpan().EndLinePosition.Line;

        return endLine - startLine + 1;
    }
}