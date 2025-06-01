using CodeAnalyzer.Core.Identifiers;
using CodeAnalyzer.Core.Models.Enums;
using CodeAnalyzer.Core.Models.Stats.Data;

namespace CodeAnalyzer.Core.Models.Builders;

public sealed class ClassModelsBuilder
{
    private readonly Dictionary<string, ClassModelBuilder> _namespaceToBuilder = [];
    
    public void RegisterClass(IdentifierDto identifier)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.WithIdentifier(identifier, ClassType.Class));
    }

    public void RegisterInterface(IdentifierDto identifier)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.WithIdentifier(identifier, ClassType.Interface));
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

    public void RegisterCbo(IdentifierDto identifier, CboDto cbo)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.AddCbo(cbo));
    }

    public void RegisterAtfd(IdentifierDto identifier, AtfdDto atfd)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.AddAtfd(atfd));
    }

    public void RegisterTcc(IdentifierDto identifier, TccDto tcc)
    {
        AddRegistry(identifier.FullName, classBuilder => classBuilder.AddTcc(tcc));
    }

    public IEnumerable<ClassModel> Build()
    {
        return _namespaceToBuilder.Select(kvp => kvp.Value.Build());
    }

    private void AddRegistry(string modelNamespace, Action<ClassModelBuilder> registryAction)
    {
        if (_namespaceToBuilder.TryGetValue(modelNamespace, out ClassModelBuilder? classBuilder))
        {
            registryAction(classBuilder);
            return;
        }
        
        ClassModelBuilder newClassBuilder = new();
        registryAction(newClassBuilder);
        _namespaceToBuilder.Add(modelNamespace, newClassBuilder);
    }
}