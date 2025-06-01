using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Builders;

public class ClassModelBuilder
{
    private readonly List<MethodModel> _methods = [];
    private readonly List<PropertyModel> _properties = [];
    private readonly List<FieldModel> _fields = [];
    private CboDto? _cbo;
    private AtfdDto? _atfd;
    private TccDto? _tcc;
    private IdentifierDto? _identifier;
    private ClassType? _classType;

    public ClassModelBuilder WithIdentifier(IdentifierDto identifier, ClassType classType)
    {
        _identifier = identifier;
        _classType = classType;
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

    public ClassModelBuilder AddCbo(CboDto cbo)
    {
        _cbo = cbo;
        return this;
    }

    public ClassModelBuilder AddAtfd(AtfdDto atfd)
    {
        _atfd = atfd;
        return this;
    }

    public ClassModelBuilder AddTcc(TccDto tcc)
    {
        _tcc = tcc;
        return this;
    }

    public ClassModel Build()
    {
        ArgumentNullException.ThrowIfNull(_identifier, nameof(_identifier));
        ArgumentNullException.ThrowIfNull(_classType, nameof(_classType));
        ClassModel model = new(_identifier, _classType.Value, _methods, _properties, _fields);

        if (_cbo is not null)
        {
            model.Stats.Cbo = _cbo;
        }

        if (_atfd is not null)
        {
            model.Stats.Atfd = _atfd;
        }

        if (_tcc is not null)
        {
            model.Stats.Tcc = _tcc;
        }

        return model;
    }
}