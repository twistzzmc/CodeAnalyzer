using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class PropertyModel(IdentifierDto identifier) : IModel
{
    public IdentifierDto Identifier { get; } = identifier;
    
    public Type? Type { get; set; }

    public override string ToString()
    {
        return $"{nameof(PropertyModel)} {Identifier}";
    }
}