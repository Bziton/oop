using System.Globalization;
using System.Text;

namespace IndependentWork12;

internal static class Program
{
    private const int MaxValue = 2_000_000;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine($"Логічних процесорів: {Environment.ProcessorCount}");
        Console.WriteLine();

        Console.WriteLine("Генерація 10 000 000 випадкових чисел...");
        List<int> allNumbers = ComputeOperations.GenerateNumbers(10_000_000, MaxValue);
        Console.WriteLine();

        PerformanceExperiments.WarmUp(allNumbers.GetRange(0, 100_000));

        Console.WriteLine("=== Частина 1: порівняння LINQ та PLINQ ===");
        var results = new List<ExperimentResult>();

        foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
        {
            List<int> data = allNumbers.GetRange(0, size);

            results.Add(PerformanceExperiments.RunHeavy(data));
            PrintResult(results[^1]);

            results.Add(PerformanceExperiments.RunLight(data));
            PrintResult(results[^1]);
        }

        Console.WriteLine();
        PrintSummary(results);

        Console.WriteLine();
        Console.WriteLine("=== Частина 2: побічні ефекти в PLINQ ===");
        SideEffectsDemo.Run(allNumbers.GetRange(0, 1_000_000));
    }

    private static void PrintResult(ExperimentResult r)
    {
        Console.WriteLine($"{r.Operation}, {r.Size:N0} елементів: LINQ {r.LinqMs:F1} мс, PLINQ {r.PlinqMs:F1} мс, прискорення {r.Speedup:F2}x (знайдено {r.LinqCount} і {r.PlinqCount})");
    }

    private static void PrintSummary(List<ExperimentResult> results)
    {
        Console.WriteLine("Операція               |    Розмір | LINQ, мс | PLINQ, мс | Прискорення | Результати збігаються");
        foreach (var r in results)
        {
            Console.WriteLine($"{r.Operation,-22} | {r.Size,9:N0} | {r.LinqMs,8:F1} | {r.PlinqMs,9:F1} | {r.Speedup,10:F2}x | {(r.Match ? "так" : "НІ")}");
        }
    }
}