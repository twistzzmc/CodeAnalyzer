using CodeAnalyzer.Core.Identifiers;

namespace CodeAnalyzer.Core.Models.Builders;

public class ClassModelBuilder
{
    private readonly List<MethodModel> _methods = [];
    private readonly List<PropertyModel> _properties = [];
    private IdentifierDto? _identifier;

    public ClassModelBuilder WithIdentifier(IdentifierDto identifier)
    {
        _identifier = identifier;
        return this;
    }

    public ClassModelBuilder AddMethod(MethodModel method)
    {
        _methods.Add(method);
        return this;
    }

    public ClassModelBuilder AddProperty(PropertyModel property)
    {
        _properties.Add(property);
        return this;
    }

    public ClassModel Build()
    {
        ArgumentNullException.ThrowIfNull(_identifier, nameof(_identifier));
        return new ClassModel(_identifier, _methods, _properties);
    }
}