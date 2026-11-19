using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IndependentWork11;

internal static class Program
{
    private const int Repeats = 1000;

    private record Timing(string Name, double FirstMs, double AverageMs);

    private static readonly List<Timing> Timings = new();

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var rnd = new Random(42);
        var products = DataGenerator.GenerateProducts(rnd);
        var customers = DataGenerator.GenerateCustomers(rnd);
        var sales = DataGenerator.GenerateSales(rnd, products.Count, customers.Count, 500);

        Console.WriteLine($"Продукти: {products.Count}, клієнти: {customers.Count}, продажі: {sales.Count}");
        Console.WriteLine();

        PrintTopProducts(Measure("Пайплайн 1", () => SalesPipelines.TopProductsByCategory(sales, products)));
        PrintRevenueByCity(Measure("Пайплайн 2", () => SalesPipelines.RevenueByCity(sales, products, customers)));
        PrintSalesByMonth(Measure("Пайплайн 3", () => SalesPipelines.SalesByMonth(sales, products)));
        PrintTopCustomers(Measure("Пайплайн 4", () => SalesPipelines.TopCustomers(sales, products, customers)));
        PrintSummary();
    }

    private static T Measure<T>(string name, Func<T> pipeline)
    {
        var firstWatch = Stopwatch.StartNew();
        T result = pipeline();
        firstWatch.Stop();

        var averageWatch = Stopwatch.StartNew();
        for (int i = 0; i < Repeats; i++)
        {
            pipeline();
        }
        averageWatch.Stop();

        Timings.Add(new Timing(
            name,
            firstWatch.Elapsed.TotalMilliseconds,
            averageWatch.Elapsed.TotalMilliseconds / Repeats));

        return result;
    }

    private static void PrintTiming()
    {
        Timing timing = Timings[^1];
        Console.WriteLine($"Час: перший запуск {timing.FirstMs:F3} мс, середній за {Repeats} запусків {timing.AverageMs:F4} мс");
        Console.WriteLine();
    }

    private static void PrintTopProducts(List<CategoryTop> result)
    {
        Console.WriteLine("--- Пайплайн 1: найпопулярніші продукти за категоріями (топ-3) ---");
        foreach (var category in result)
        {
            Console.WriteLine($"{category.Category}:");
            int place = 1;
            foreach (var product in category.TopProducts)
            {
                Console.WriteLine($"  {place}. {product.Name}, {product.TotalQuantity} шт.");
                place++;
            }
        }
        PrintTiming();
    }

    private static void PrintRevenueByCity(List<CityRevenue> result)
    {
        Console.WriteLine("--- Пайплайн 2: загальний дохід за містами клієнтів ---");
        foreach (var city in result)
        {
            Console.WriteLine($"  {city.City}: {city.TotalRevenue:F2} грн ({city.SalesCount} продажів)");
        }
        PrintTiming();
    }

    private static void PrintSalesByMonth(List<MonthlySales> result)
    {
        Console.WriteLine("--- Пайплайн 3: продажі за місяцями та роками ---");
        foreach (var month in result)
        {
            Console.WriteLine($"  {month.Year}-{month.Month:D2}: {month.TotalQuantity} шт., {month.TotalRevenue:F2} грн");
        }
        PrintTiming();
    }

    private static void PrintTopCustomers(List<TopCustomer> result)
    {
        Console.WriteLine("--- Пайплайн 4 (додатково): топ-5 клієнтів за витратами ---");
        int place = 1;
        foreach (var customer in result)
        {
            Console.WriteLine($"  {place}. {customer.Name} ({customer.City}): {customer.TotalSpent:F2} грн, улюблена категорія: {customer.FavoriteCategory}");
            place++;
        }
        PrintTiming();
    }

    private static void PrintSummary()
    {
        Console.WriteLine("--- Підсумок вимірювань ---");
        foreach (var timing in Timings)
        {
            Console.WriteLine($"  {timing.Name}: перший запуск {timing.FirstMs:F3} мс, середній {timing.AverageMs:F4} мс");
        }
    }
}