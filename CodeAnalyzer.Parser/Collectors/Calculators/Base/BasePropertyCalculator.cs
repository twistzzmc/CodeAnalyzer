using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Parser.Collectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzer.Parser.Collectors.Calculators.Base;

internal abstract class BasePropertyCalculator<TPropertyValue> :
    ICalculator<TPropertyValue, CSharpSyntaxNode>,
    ICalculator<IPropertyValue<TPropertyValue>, PropertyDeclarationSyntax>
{

    public abstract TPropertyValue Calculate(CSharpSyntaxNode options);
    
    public IPropertyValue<TPropertyValue> Calculate(PropertyDeclarationSyntax options)
    {
        if (options.AccessorList is null)
        {
            return Create(default, default);
        }
        
        TPropertyValue? getterComplexity = default;
        TPropertyValue? setterComplexity = default;

        foreach (AccessorDeclarationSyntax accessor in options.AccessorList.Accessors)
        {
            if (accessor.Kind() == SyntaxKind.GetAccessorDeclaration)
            {
                getterComplexity = Calculate(accessor);
            }
            
            if (accessor.Kind() == SyntaxKind.SetAccessorDeclaration)
            {
                setterComplexity = Calculate(accessor);
            }
        }
        
        return Create(getterComplexity, setterComplexity);
    }
    
    protected abstract IPropertyValue<TPropertyValue> Create(TPropertyValue? get, TPropertyValue? set);
}