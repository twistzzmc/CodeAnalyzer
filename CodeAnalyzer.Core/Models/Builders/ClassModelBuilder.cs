using System.Collections.Concurrent;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Builders;

public class ClassModelBuilder
{
    private struct Snapshot
    {
        public required MethodModel[] Methods { get; init; }
        public required PropertyModel[] Properties { get; init; }
        public required FieldModel[] Fields { get; init; }
        
        public required CboDto? Cbo { get; init; }
        public required AtfdDto? Atfd { get; init; }
        public required TccDto? Tcc { get; init; }
        
        public required IdentifierDto? Identifier { get; init; }
        public required ClassType? ClassType { get; init; }
    }
    
    private readonly Lock _lock = new();
    
    private readonly ConcurrentBag<MethodModel> _methods = [];
    private readonly ConcurrentBag<PropertyModel> _properties = [];
    private readonly ConcurrentBag<FieldModel> _fields = [];
    
    private CboDto? _cbo;
    private AtfdDto? _atfd;
    private TccDto? _tcc;
    private IdentifierDto? _identifier;
    private ClassType? _classType;

    public ClassModelBuilder WithIdentifier(IdentifierDto identifier, ClassType classType)
    {
        lock (_lock)
        {
            _identifier = identifier;
            _classType = classType;
        }
        
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
        lock (_lock)
        {
            _cbo = cbo;
        }
        
        return this;
    }

    public ClassModelBuilder AddAtfd(AtfdDto atfd)
    {
        lock (_lock)
        {
            _atfd = atfd;
        }
        
        return this;
    }

    public ClassModelBuilder AddTcc(TccDto tcc)
    {
        lock (_lock)
        {
            _tcc = tcc;
        }
        
        return this;
    }

    public ClassModel Build()
    {
        Snapshot snap = TakeSnapshot();
        
        ArgumentNullException.ThrowIfNull(snap.Identifier, nameof(snap.Identifier));
        ArgumentNullException.ThrowIfNull(snap.ClassType, nameof(snap.ClassType));
        
        ClassModel model = new(snap.Identifier, snap.ClassType.Value, snap.Methods, snap.Properties, snap.Fields);

        if (snap.Cbo is not null)
        {
            model.Stats.Cbo = snap.Cbo;
        }

        if (snap.Atfd is not null)
        {
            model.Stats.Atfd = snap.Atfd;
        }

        if (snap.Tcc is not null)
        {
            model.Stats.Tcc = snap.Tcc;
        }

        return model;
    }

    internal void DumpData(ILogger logger)
    {
        Snapshot snap = TakeSnapshot();
        
        logger.Info($"Identifier: {snap.Identifier}");
        logger.Info($"ClassType: {snap.ClassType}");
        logger.Info($"Cbo: {snap.Cbo}");
        logger.Info($"Atfd: {snap.Atfd}");
        logger.Info($"Tcc: {snap.Tcc}");

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

    private Snapshot TakeSnapshot()
    {
        lock (_lock)
        {
            return new Snapshot
            {
                Methods = _methods.ToArray(),
                Properties = _properties.ToArray(),
                Fields = _fields.ToArray(),
                
                Cbo = _cbo,
                Atfd = _atfd,
                Tcc = _tcc,
                
                Identifier = _identifier,
                ClassType = _classType,
            };
        }
    }
}