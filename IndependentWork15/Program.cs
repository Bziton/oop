using System.Globalization;
using System.Text;

namespace Lab9SelfV2;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        DemoNumbers();
        Console.WriteLine();
        DemoWords();
        Console.WriteLine();
        DemoProducts();
    }

    private static void DemoNumbers()
    {
        Console.WriteLine("--- Колекція 1: List<int> ---");
        var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var squares = CollectionUtils.Map(numbers, n => n * n);
        Console.WriteLine("Map (квадрати): " + string.Join(", ", squares));

        var evens = CollectionUtils.Filter(numbers, n => n % 2 == 0);
        Console.WriteLine("Filter (парні): " + string.Join(", ", evens));

        var farthest = CollectionUtils.MaxBy(numbers, n => Math.Abs(n - 5));
        Console.WriteLine($"MaxBy (найдальше від 5): {farthest}");

        int sum = 0;
        CollectionUtils.ForEach(numbers, n => sum += n);
        Console.WriteLine($"ForEach (сума елементів): {sum}");
    }

    private static void DemoWords()
    {
        Console.WriteLine("--- Колекція 2: List<string> ---");
        var words = new List<string> { "apple", "banana", "kiwi", "cherry", "fig", "pineapple" };

        var upper = CollectionUtils.Map(words, w => w.ToUpperInvariant());
        Console.WriteLine("Map (великі літери): " + string.Join(", ", upper));

        var longWords = CollectionUtils.Filter(words, w => w.Length > 4);
        Console.WriteLine("Filter (довші за 4 символи):");
        CollectionUtils.ForEach(longWords, w => Console.WriteLine($"  - {w}"));

        var longest = CollectionUtils.MaxBy(words, w => w.Length);
        Console.WriteLine($"MaxBy (найдовше слово): {longest}");
    }

    private static void DemoProducts()
    {
        Console.WriteLine("--- Колекція 3: List<Product> ---");
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 32000 },
            new Product { Name = "Mouse", Price = 550 },
            new Product { Name = "Keyboard", Price = 1200 },
            new Product { Name = "Monitor", Price = 8500 },
            new Product { Name = "Webcam", Price = 1700 }
        };

        var names = CollectionUtils.Map(products, p => p.Name);
        Console.WriteLine("Map (назви): " + string.Join(", ", names));

        var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine($"MaxBy (найдорожчий): {mostExpensive.Name}, {mostExpensive.Price:F2} грн");

        var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
        Console.WriteLine("Filter (дорожчі за 1000 грн):");
        CollectionUtils.ForEach(expensive, p => Console.WriteLine($"  - {p.Name}: {p.Price:F2} грн"));

        var discounted = CollectionUtils.Map(products, p => new { p.Name, Price = p.Price * 0.9m });
        Console.WriteLine("Map + ForEach (ціни зі знижкою 10%):");
        CollectionUtils.ForEach(discounted, d => Console.WriteLine($"  {d.Name}: {d.Price:F2} грн"));
    }
}