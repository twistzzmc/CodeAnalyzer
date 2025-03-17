using System.Text;
using CodeAnalyzer.Core.Models.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class ClassModel(string id, string name, IEnumerable<MethodModel> methods, IEnumerable<PropertyModel> properties)
    : IModel
{
    private static readonly string ListDelimeter = $"{Environment.NewLine}\t\t";

    public string Id => id;
    public string Name => name;
    public IReadOnlyList<MethodModel> Methods => methods.ToList();
    public IReadOnlyList<PropertyModel> Properties => properties.ToList();

    public override string ToString()
    {
        return new StringBuilder($"[{Id}] ")
            .AppendLine($"{nameof(ClassModel)}(")
            .AppendLine($"\t{nameof(Name)}: {Name}")
            .Append($"\t[{Methods.Count}] {nameof(Methods)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Methods))
            .Append($"\t[{Properties.Count}] {nameof(Properties)}:")
            .Append(ListDelimeter)
            .AppendLine(string.Join(ListDelimeter, Properties))
            .ToString();
    }

    public void PrintTooLongMethods()
    {
        IOrderedEnumerable<MethodModel> orderedMethods = Methods.OrderBy(m => m.Length * -1);
        
        foreach (MethodModel method in orderedMethods)
        {
            if (method is { Length: > 100, CyclomaticComplexity: > 10 })
            {
                Console.WriteLine($"Metoda {method.Name} jest za duża (linia: {method.LineStart}).");
                Console.WriteLine($"\t Liczba linii: {method.Length}");
                Console.WriteLine($"\t Złożoność cyklometryczna: {method.CyclomaticComplexity}");
                Console.WriteLine("\t Zaleca się podzielić metode na kilka mniejszych");
                Console.WriteLine();
            }
            else if (method is { Length: > 50, CyclomaticComplexity: > 5 })
            {
                Console.WriteLine($"Metoda {method.Name} może być za duża (linia: {method.LineStart}).");
                Console.WriteLine($"\t Liczba linii: {method.Length}");
                Console.WriteLine($"\t Złożoność cyklometryczna: {method.CyclomaticComplexity}");
                Console.WriteLine("\t Podzielenie metody może pozytywnie wpłynąć na czytelność kodu");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Metoda {method.Name} ma dobrą długość. (linia: {method.LineStart}, długość: {method.Length}, złożonoś cyklometryczna: {method.CyclomaticComplexity}).");
            }
        }
    }
}