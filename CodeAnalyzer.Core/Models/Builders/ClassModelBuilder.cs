using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Interfaces;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Builders;

public class ClassModelBuilder : IFillable<ClassModelBuilder>
{
    private readonly List<MethodModel> _methods = [];
    private readonly List<PropertyModel> _properties = [];
    private readonly List<FieldModel> _fields = [];
    
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
    
    public void Fill(ClassModelBuilder other)
    {
        if (_identifier is not null && _classType.HasValue)
        {
            other.WithIdentifier(_identifier, _classType.Value);
        }

        if (_atfd is not null)
        {
            AtfdDto atfd = other._atfd is not null
                ? _atfd.Join(other._atfd)
                : _atfd;
            
            other.AddAtfd(atfd);
        }

        if (_tcc is not null)
        {
            TccDto tcc = other._tcc is not null
                ? _tcc.Join(other._tcc)
                : _tcc;
            
            other.AddTcc(tcc);
        }

        foreach (MethodModel method in _methods)
        {
            other.AddMethod(method);
        }

        foreach (PropertyModel property in _properties)
        {
            other.AddProperty(property);
        }

        foreach (FieldModel field in _fields)
        {
            other.AddField(field);
        }
    }

    internal void DumpData(ILogger logger)
    {
        logger.Info($"Identifier: {_identifier}");
        logger.Info($"ClassType: {_classType}");
        logger.Info($"Atfd: {_atfd}");
        logger.Info($"Tcc: {_tcc}");

        try
        {
            logger.OpenLevel($"[{_methods.Count}] Methods");
            
            foreach (MethodModel methodModel in _methods)
            {
                logger.Info($"{methodModel.Identifier}");
                logger.Info($"[{methodModel.LineStart}] {methodModel.ReturnType}");
            }

            foreach (PropertyModel propertyModel in _properties)
            {
                logger.Info($"{propertyModel.Identifier}");
                logger.Info($"[{propertyModel.LineStart}] {propertyModel.Type}");
            }

            foreach (FieldModel fieldModel in _fields)
            {
                logger.Info($"{fieldModel.Identifier}");
                logger.Info($"[{fieldModel.LineStart}] {fieldModel.Type}");
            }
        }
        finally
        {
            logger.CloseLevel();
        }
    }
}