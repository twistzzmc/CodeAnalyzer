using System.Collections.Concurrent;
using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Interfaces;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Builders;

public sealed class ClassModelsBuilder : IFillable<ClassModelsBuilder>
{
    private readonly ConcurrentDictionary<string, ClassModelBuilder> _namespaceToBuilder = [];
    
    public void RegisterClass(IdentifierDto identifier)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.WithIdentifier(identifier, ClassType.Class));
    }

    public void RegisterInterface(IdentifierDto identifier)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.WithIdentifier(identifier, ClassType.Interface));
    }

    public void RegisterStruct(IdentifierDto identifier)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.WithIdentifier(identifier, ClassType.Struct));
    }

    public void RegisterMethod(MethodModel method)
    {
        AddRegistry(method.Identifier.NamespaceText, classBuilder => classBuilder.AddMethod(method));
    }

    public void RegisterProperty(PropertyModel property)
    {
        AddRegistry(property.Identifier.NamespaceText, classBuilder => classBuilder.AddProperty(property));
    }

    public void RegisterField(FieldModel field)
    {
        AddRegistry(field.Identifier.NamespaceText, classBuilder => classBuilder.AddField(field));
    }

    public void RegisterAtfd(IdentifierDto identifier, AtfdDto atfd)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.AddAtfd(atfd));
    }

    public void RegisterTcc(IdentifierDto identifier, TccDto tcc)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.AddTcc(tcc));
    }

    public IEnumerable<ClassModel> Build(ILogger logger)
    {
        List<ClassModel> models = [];

        foreach (KeyValuePair<string, ClassModelBuilder> kvp in _namespaceToBuilder)
        {
            try
            {
                models.Add(kvp.Value.Build());
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                kvp.Value.DumpData(logger);
            }
        }

        return models;
    }

    public void Fill(ClassModelsBuilder other)
    {
        foreach (KeyValuePair<string, ClassModelBuilder> kvp in _namespaceToBuilder)
        {
            ClassModelBuilder? builder = null;
            bool builderExists = false;

            if (other._namespaceToBuilder.TryGetValue(kvp.Key, out ClassModelBuilder? existingBuilder))
            {
                builder = existingBuilder;
                builderExists = true;
            }
            
            builder ??= new ClassModelBuilder();
            kvp.Value.Fill(builder);

            if (!builderExists)
            {
                other._namespaceToBuilder.TryAdd(kvp.Key, builder);
            }
        }
    }

    private void AddRegistry(string modelNamespace, Action<ClassModelBuilder> registryAction)
    {
        if (TryRegisterOnExistingNamespace(modelNamespace, registryAction))
        {
            return;
        }
        
        ClassModelBuilder newClassBuilder = new();
        registryAction(newClassBuilder);
        if (_namespaceToBuilder.TryAdd(modelNamespace, newClassBuilder))
        {
            return;
        }
        
        if (!TryRegisterOnExistingNamespace(modelNamespace, registryAction))
        {
            throw new InvalidOperationException("Nie udało się dodać ani zaktualizować wpisu");
        }
    }

    private bool TryRegisterOnExistingNamespace(string modelNamespace, Action<ClassModelBuilder> registryAction)
    {
        if (!_namespaceToBuilder.TryGetValue(modelNamespace, out ClassModelBuilder? classBuilder))
        {
            return false;
        }
        
        registryAction(classBuilder);
        return true;
    }
}