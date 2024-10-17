namespace CodeAnalyzer.Core.Identifiers;

public static class IdentifierCreator
{
    public static string Create()
    {
        return Guid.NewGuid().ToString("N");
    }
}