using CodeAnalyzer.Core.Identifiers;

namespace CodeAnalyzer.Core.Models.Interfaces;

public interface IModel
{
    IdentifierDto Identifier { get; }
}