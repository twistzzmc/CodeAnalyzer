namespace CodeAnalyzer.Core.Models.Enums;

public enum AccessModifierType
{
    Unknown = -1,
    None = 0,
    Public = 1,
    ProtectedInternal,
    Protected,
    Internal,
    PrivateProtected,
    Private
}