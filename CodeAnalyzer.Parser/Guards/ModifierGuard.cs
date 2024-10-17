using CodeAnalyzer.Core.Constants;
using CodeAnalyzer.Core.Warnings.Enums;
using CodeAnalyzer.Core.Warnings.Interfaces;

namespace CodeAnalyzer.Parser.Guards;

public sealed class ModifierGuard(IWarningRegistry warningRegistry)
{
    private static readonly Lazy<IReadOnlyList<string>> AllowedModifiers = new(() => new List<string>()
    {
        Modifiers.PUBLIC,
        Modifiers.PROTECTED,
        Modifiers.INTERNAL,
        Modifiers.PRIVATE,
        Modifiers.STATIC,
        Modifiers.FINAL,
        Modifiers.ASYNC,
        Modifiers.VIRTUAL,
        Modifiers.ABSTRACT
    });

    public void GuardAgainstUnknown(IEnumerable<string> modifiers)
    {
        List<string> unknownModifiers = new();
        
        foreach (string modifier in modifiers)
        {
            if (AllowedModifiers.Value.Any(m => m == modifier))
            {
                continue;
            }
            
            unknownModifiers.Add(modifier);
        }

        if (unknownModifiers.Count == 0)
        {
            return;
        }
        
        warningRegistry.RegisterWarning(WarningType.UnknownModifier,
            $"Found unknown modifier '{string.Join(", ", unknownModifiers)}'");
    }

    public static void GuardAgainstUnknown(IWarningRegistry warningRegistry, IEnumerable<string> modifiers)
    {
        new ModifierGuard(warningRegistry).GuardAgainstUnknown(modifiers);
    }
}