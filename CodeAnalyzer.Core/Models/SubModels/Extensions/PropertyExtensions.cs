using CodeAnalyzer.Core.Models.Interfaces;
using CodeAnalyzer.Core.Models.SubModels.PropertyValues;

namespace CodeAnalyzer.Core.Models.SubModels.Extensions;

public static class PropertyExtensions
{
    public static PropertyCyclomaticComplexity ToComplexityDto(this IPropertyValue<int> propertyValue)
    {
        return new PropertyCyclomaticComplexity(propertyValue.Get, propertyValue.Set);
    }
    
    public static PropertyLength ToLengthDto(this IPropertyValue<int> propertyValue)
    {
        return new PropertyLength(propertyValue.Get, propertyValue.Set);
    }
}