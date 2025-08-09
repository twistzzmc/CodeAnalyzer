using CodeAnalyzer.Core.Logging.Enums;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Parser.Collectors.Calculators.Interfaces;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Parser.Collectors.Calculators;

public sealed class AccessModifierCalculator(IWarningRegistry warningRegistry)
    : ICalculator<AccessModifierType, SyntaxTokenList>
{
    private const string PUBLIC = "Public";
    private const string INTERNAL = "Internal";
    private const string PROTECTED = "Protected";
    private const string PRIVATE = "Private";
    
    private bool _isPublic;
    private bool _isInternal;
    private bool _isProtected;
    private bool _isPrivate;
    
    private readonly List<string> _modifiers = [];
    
    public AccessModifierType Calculate(SyntaxTokenList modifierList)
    {
        _modifiers.Clear();

        FindModifiers(modifierList);

        return ConvertByFoundModifiers();
    }

    private void FindModifiers(SyntaxTokenList modifierList)
    {
        foreach (SyntaxToken modifier in modifierList)
        {
            if (CheckModifier(ref _isPublic, PUBLIC, modifier.Text))
            {
                continue;
            }

            if (CheckModifier(ref _isInternal, INTERNAL, modifier.Text))
            {
                continue;
            }

            if (CheckModifier(ref _isProtected, PROTECTED, modifier.Text))
            {
                continue;
            }

            CheckModifier(ref _isPrivate, PRIVATE, modifier.Text);
        }
    }

    private AccessModifierType ConvertByFoundModifiers()
    {
        if (_isPublic && !_isPrivate && !_isProtected && !_isInternal)
        {
            return AccessModifierType.Public;
        }

        if (_isProtected && _isInternal && !_isPrivate && !_isPublic)
        {
            return AccessModifierType.ProtectedInternal;
        }

        if (_isProtected && !_isInternal && !_isPrivate && !_isPublic)
        {
            return AccessModifierType.Protected;
        }

        if (_isInternal && !_isPrivate && !_isProtected && !_isPublic)
        {
            return AccessModifierType.Internal;
        }
        
        if (_isPrivate && _isProtected && !_isPublic && !_isInternal)
        {
            return AccessModifierType.PrivateProtected;
        }

        if (_isPrivate && !_isPublic && !_isProtected && !_isInternal)
        {
            return AccessModifierType.Private;
        }

        if (!_isPrivate && !_isPublic && !_isProtected && !_isInternal)
        {
            return AccessModifierType.None;
        }
        
        RegisterIncorrectModifiersWarning(_modifiers.ToArray());
        return AccessModifierType.Unknown;
    }

    private bool CheckModifier(ref bool isThisModifier, string expectedModifier, string modifier)
    {
        if (!Compare(expectedModifier, modifier))
        {
            return false;
        }
        
        _modifiers.Add(modifier);

        if (isThisModifier)
        {
            RegisterDuplicateModifierWarning(modifier);
            return true;
        }
        
        isThisModifier = true;
        return true;
    }

    private void RegisterDuplicateModifierWarning(string duplicateModifier)
    {
        warningRegistry.RegisterWarning(WarningType.MethodModifier,
            $"Znaleziono duplikat: '{duplicateModifier}'");
    }

    private void RegisterIncorrectModifiersWarning(params string[] incorrectModifiers)
    {
        warningRegistry.RegisterWarning(WarningType.MethodModifier,
            $"Znaleziono niespodziewane parowanie modyfikatorów: '{string.Join(", ", incorrectModifiers)}'");
    }

    private static bool Compare(string expectedModifier, string modifier)
    {
        return string.Equals(expectedModifier, modifier, StringComparison.InvariantCultureIgnoreCase);
    }
}