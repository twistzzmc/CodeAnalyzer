namespace CodeAnalyzer.Core.Models.Interfaces;

public interface IPropertyValue<out TValue>
{
    TValue Get { get; }
    TValue Set { get; }
}