using CodeAnalyzer.Core.Constants;
using CodeAnalyzer.Core.Logging.Enums;
using CodeAnalyzer.Core.Logging.Interfaces;

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
        Modifiers.ABSTRACT,
        Modifiers.OVERRIDE,
        Modifiers.EXTERN,
        Modifiers.NEW,
        Modifiers.SEALED
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
        
        warningRegistry.RegisterWarning(WarningType.MethodModifier,
            $"Znaleziono nieznany modyfikator '{string.Join(", ", unknownModifiers)}'");
    }

    public static void GuardAgainstUnknown(IWarningRegistry warningRegistry, IEnumerable<string> modifiers)
    {
        new ModifierGuard(warningRegistry).GuardAgainstUnknown(modifiers);
    }
}