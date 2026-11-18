using System.Text;

namespace IndependentWork7;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("--- Демонстрація Cache<string, string> з ємністю 3 ---");
        var cache = new Cache<string, string>(3);

        cache.Add("ua", "Ukraine");
        cache.Add("pl", "Poland");
        cache.Add("de", "Germany");
        Console.WriteLine($"Додано: ua, pl, de. Кількість елементів: {cache.Count}");
        Console.WriteLine($"Get(\"pl\") = {cache.Get("pl")}");

        cache.Add("fr", "France");
        Console.WriteLine("Додаємо fr, кеш переповнений, найстаріший елемент (ua) видалено");
        Console.WriteLine($"ContainsKey(\"ua\") = {cache.ContainsKey("ua")}");
        Console.WriteLine($"ContainsKey(\"fr\") = {cache.ContainsKey("fr")}");

        try
        {
            cache.Get("ua");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Get(\"ua\"): {ex.Message}");
        }

        cache.Add("it", "Italy");
        Console.WriteLine("Додаємо it, видалено найстаріший (pl), хоча його щойно читали (FIFO)");
        Console.WriteLine($"ContainsKey(\"pl\") = {cache.ContainsKey("pl")}");
        Console.WriteLine($"ContainsKey(\"de\") = {cache.ContainsKey("de")}");
        Console.WriteLine($"Кількість елементів: {cache.Count}");
    }
}