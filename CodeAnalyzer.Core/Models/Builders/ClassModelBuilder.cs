using CodeAnalyzer.Core.Identifiers;

namespace CodeAnalyzer.Core.Models.Builders;

public class ClassModelBuilder
{
    private readonly List<MethodModel> _methods = [];
    private readonly List<PropertyModel> _properties = [];
    private readonly List<FieldModel> _fields = [];
    private int? _cbo;
    private int? _atfd;
    private double? _tcc;
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

    public ClassModelBuilder AddField(FieldModel field)
    {
        _fields.Add(field);
        return this;
    }

    public ClassModelBuilder AddCbo(int cbo)
    {
        _cbo = cbo;
        return this;
    }

    public ClassModelBuilder AddAtfd(int atfd)
    {
        _atfd = atfd;
        return this;
    }

    public ClassModelBuilder AddTcc(double tcc)
    {
        _tcc = tcc;
        return this;
    }

    public ClassModel Build()
    {
        ArgumentNullException.ThrowIfNull(_identifier, nameof(_identifier));
        ClassModel model = new(_identifier, _methods, _properties, _fields);

        if (_cbo.HasValue)
        {
            model.Stats.Cbo = _cbo.Value;
        }

        if (_atfd.HasValue)
        {
            model.Stats.Atfd = _atfd.Value;
        }

        if (_tcc.HasValue)
        {
            model.Stats.Tcc = _tcc.Value;
        }

        return model;
    }
}