namespace CodeAnalyzer.Models.Builders;

public class ClassModelBuilder
{
    private readonly List<MethodModel> _methods = [];
    private readonly List<PropertyModel> _properties = [];

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
        return new ClassModel(_methods, _properties);
    }
}