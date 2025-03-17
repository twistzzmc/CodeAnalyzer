using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels;

namespace CodeAnalyzer.Core.Models;

public class MethodModel(
    string id,
    string name,
    AccessModifierType accessModifierType,
    ReturnType returnType,
    int lineStart,
    int length,
    int cyclomaticComplexity) : IModel
{
    public string Id => id;
    public string Name => name;
    public AccessModifierType AccessModifierType => accessModifierType;
    public ReturnType ReturnType => returnType;
    public int LineStart => lineStart;
    public int Length => length;
    public int CyclomaticComplexity => cyclomaticComplexity;

    public override string ToString()
    {
        return $"[{Id}] {nameof(MethodModel)}({nameof(Name)}: {Name}, " +
               $"{nameof(AccessModifierType)}: {AccessModifierType}, " +
               $"{nameof(ReturnType)}: {ReturnType}, " +
               $"{nameof(LineStart)}: {LineStart}, " +
               $"{nameof(Length)}: {Length}, " +
               $"{nameof(CyclomaticComplexity)}: {CyclomaticComplexity})";
    }
}