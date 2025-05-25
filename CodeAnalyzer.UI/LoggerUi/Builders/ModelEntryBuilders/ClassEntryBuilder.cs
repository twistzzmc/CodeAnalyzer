using CodeAnalyzer.Core.Models;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Interfaces;

namespace CodeAnalyzer.UI.LoggerUi.Builders.ModelEntryBuilders;

internal sealed class ClassEntryBuilder(
    IModelEntryBuilder<MethodModel> methodEntryBuilder,
    IModelEntryBuilder<PropertyModel> propertyEntryBuilder,
    IModelEntryBuilder<FieldModel> fieldEntryBuilder)
    : IModelEntryBuilder<ClassModel>
{
    public ClassEntryBuilder()
        : this(new MethodEntryBuilder(), new PropertyEntryBuilder(), new FieldEntryBuilder())
    { }
    
    public LogEntry Build(ClassModel model)
    {
        LogEntry entry = new(model.Identifier.ToString());
        LogEntry methods = new($"[{model.Methods.Count}] Metody");
        LogEntry properties = new($"[{model.Properties.Count}] Właściwości");
        LogEntry fields = new($"[{model.Fields.Count}] Pola");
        
        entry.AddChild(methods);
        entry.AddChild(properties);
        entry.AddChild(fields);

        foreach (MethodModel method in model.Methods)
        {
            methods.AddChild(methodEntryBuilder.Build(method));
        }

        foreach (PropertyModel property in model.Properties)
        {
            properties.AddChild(propertyEntryBuilder.Build(property));
        }

        foreach (FieldModel field in model.Fields)
        {
            fields.AddChild(fieldEntryBuilder.Build(field));
        }
        
        return entry;
    }
}