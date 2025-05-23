namespace CodeAnalyzer.Core.Identifiers;

public sealed class NamespacePartDto
{
    public string Value { get; }
    public bool IsClass { get; }
    public bool IsStruct { get; }
    public bool IsRecord { get; }
    public bool IsInterface { get; }
    
    public bool IsStatic { get; }
    public bool IsPartial { get; }

    private NamespacePartDto(
        string value,
        bool isClass,
        bool isStruct,
        bool isRecord,
        bool isInterface,
        bool isStatic,
        bool isPartial)
    {
        Value = value;
        IsClass = isClass;
        IsStruct = isStruct;
        IsRecord = isRecord;
        IsInterface = isInterface;
        IsStatic = isStatic;
        IsPartial = isPartial;
    }

    public static NamespacePartDto FromClass(string value, bool isStatic, bool isPartial)
    {
        return new NamespacePartDto(value, true, false, false, false, isStatic, isPartial);
    }

    public static NamespacePartDto FromPure(string value)
    {
        return new NamespacePartDto(value, false, false, false,false, false, false);
    }

    public static NamespacePartDto FromStruct(string value, bool isPartial)
    {
        return new NamespacePartDto(value, false, true, false, false, false, isPartial);
    }

    public static NamespacePartDto FromRecord(string value, bool isPartial)
    {
        return new NamespacePartDto(value, true, false, true, false, false, isPartial);
    }

    public static NamespacePartDto FromRecordStruct(string value, bool isPartial)
    {
        return new NamespacePartDto(value, false, true, true, false, false, isPartial);
    }

    public static NamespacePartDto FromInterface(string value, bool isPartial)
    {
        return new NamespacePartDto(value, false, false, false, true, false, isPartial);
    }

    public static NamespacePartDto FromBlock(string value)
    {
        return new NamespacePartDto(value, false, false, false, true, false, false);
    }
}