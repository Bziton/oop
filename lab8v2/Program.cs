using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== List<int> -> List<string> ===");
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
        List<string> numberLabels = new List<string>(CollectionHelper<int>.Map(numbers, n => $"Число #{n}"));
        Console.WriteLine($"Вхідні дані: {string.Join(", ", numbers)}");
        Console.WriteLine($"Результат: {string.Join(", ", numberLabels)}");

        Console.WriteLine();
        Console.WriteLine("=== List<int> -> List<int> (квадрати) ===");
        List<int> squares = new List<int>(CollectionHelper<int>.Map(numbers, n => n * n));
        Console.WriteLine($"Результат: {string.Join(", ", squares)}");

        Console.WriteLine();
        Console.WriteLine("=== List<string> -> List<int> (довжини рядків) ===");
        List<string> words = new List<string> { "Apple", "Banana", "Kiwi", "Watermelon" };
        List<int> lengths = new List<int>(CollectionHelper<string>.Map(words, w => w.Length));
        Console.WriteLine($"Вхідні дані: {string.Join(", ", words)}");
        Console.WriteLine($"Результат: {string.Join(", ", lengths)}");

        Console.WriteLine();
        Console.WriteLine("=== List<Product> -> List<decimal> (ціни) ===");
        List<Product> products = new List<Product>
        {
            new Product(1, "Ноутбук", "Електроніка", 32500m),
            new Product(2, "Миша", "Електроніка", 450m),
            new Product(3, "Книга", "Книги", 280m),
            new Product(4, "Навушники", "Електроніка", 1999.99m)
        };

        foreach (Product product in products)
        {
            Console.WriteLine(product);
        }

        List<decimal> prices = new List<decimal>(CollectionHelper<Product>.Map(products, p => p.Price));
        Console.WriteLine($"Ціни: {string.Join(", ", prices)}");

        Console.WriteLine();
        Console.WriteLine("=== List<Product> -> List<string> (назви великими літерами) ===");
        IEnumerable<string> names = CollectionHelper<Product>.Map(products, p => p.Name.ToUpper());
        Console.WriteLine($"Результат: {string.Join(", ", names)}");

        Console.WriteLine();
        Console.WriteLine("=== Порожня колекція ===");
        List<int> empty = new List<int>();
        List<string> emptyResult = new List<string>(CollectionHelper<int>.Map(empty, n => n.ToString()));
        Console.WriteLine($"Кількість елементів у результаті: {emptyResult.Count}");

        Console.WriteLine();
        Console.WriteLine("=== Відкладене виконання ===");
        int calls = 0;
        IEnumerable<int> lazy = CollectionHelper<int>.Map(numbers, n =>
        {
            calls++;
            return n * 10;
        });
        Console.WriteLine($"Після виклику Map лямбда виконалась разів: {calls}");
        List<int> materialized = new List<int>(lazy);
        Console.WriteLine($"Після перебору колекції лямбда виконалась разів: {calls}");
        Console.WriteLine($"Результат: {string.Join(", ", materialized)}");

        Console.WriteLine();
        Console.WriteLine("=== Перевірка null ===");
        try
        {
            CollectionHelper<int>.Map<string>(numbers, null!);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Виняток: {ex.ParamName}");
        }
    }
}