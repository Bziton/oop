using System.Text;
using IndependentWork7;

class Program
{
    static void PrintCache(Cache<string, string> cache, string[] keys)
    {
        Console.Write($"Кеш ({cache.Count}/{cache.Capacity}): ");
        foreach (string key in keys)
        {
            if (cache.ContainsKey(key))
            {
                Console.Write($"[{key}] ");
            }
        }
        Console.WriteLine();
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string[] allKeys = { "ua", "pl", "de", "fr", "it" };
        Cache<string, string> capitals = new Cache<string, string>(3);

        Console.WriteLine("Додаємо 3 елементи в кеш ємністю 3:");
        capitals.Add("ua", "Київ");
        capitals.Add("pl", "Варшава");
        capitals.Add("de", "Берлін");
        PrintCache(capitals, allKeys);

        Console.WriteLine($"Get(\"pl\") = {capitals.Get("pl")}");
        Console.WriteLine();

        Console.WriteLine("Додаємо \"fr\" - кеш повний, видаляється найстаріший (\"ua\"):");
        capitals.Add("fr", "Париж");
        PrintCache(capitals, allKeys);
        Console.WriteLine($"ContainsKey(\"ua\") = {capitals.ContainsKey("ua")}");
        Console.WriteLine($"ContainsKey(\"fr\") = {capitals.ContainsKey("fr")}");
        Console.WriteLine();

        Console.WriteLine("Додаємо \"it\" - видаляється \"pl\":");
        capitals.Add("it", "Рим");
        PrintCache(capitals, allKeys);
        Console.WriteLine();

        Console.WriteLine("Get(\"ua\") для видаленого ключа:");
        try
        {
            capitals.Get("ua");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"KeyNotFoundException: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("Cache<int, double> - інші типи ключа і значення:");
        Cache<int, double> squares = new Cache<int, double>(2);
        for (int i = 1; i <= 4; i++)
        {
            squares.Add(i, Math.Sqrt(i));
        }
        Console.WriteLine($"Count = {squares.Count}, є 1: {squares.ContainsKey(1)}, є 4: {squares.ContainsKey(4)}, sqrt(4) = {squares.Get(4)}");
    }
}
