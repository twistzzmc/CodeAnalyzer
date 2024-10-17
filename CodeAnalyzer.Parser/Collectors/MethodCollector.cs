using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.SubModels;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;
using CodeAnalyzer.Parser.Converters;
using CodeAnalyzer.Parser.Guards;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors;

internal sealed class MethodCollector(IWarningRegistry warningRegistry)
    : BaseCollector<MethodModel, MethodDeclarationSyntax>(warningRegistry)
{
    private readonly AccessModifierConverter _accessModifierConverter = new(warningRegistry);
    private readonly ReturnTypeConverter _returnTypeConverter = new();

    protected override ModelType CollectorType => ModelType.Method;

    protected override MethodModel InnerCollect(MethodDeclarationSyntax node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(CurrentModelIdentifier);
        
        ModifierGuard.GuardAgainstUnknown(warningRegistry, node.Modifiers.Select(m => m.Text));
        
        AccessModifierType modifier = _accessModifierConverter.Convert(node.Modifiers);
        ReturnType returnType = _returnTypeConverter.Convert(node.ReturnType);
        return new MethodModel(CurrentModelIdentifier, node.Identifier.Text, modifier, returnType);
    }
}