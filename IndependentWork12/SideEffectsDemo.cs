using System.Collections.Concurrent;
using System.Diagnostics;

namespace IndependentWork12;

public static class SideEffectsDemo
{
    private const int Attempts = 5;

    public static void Run(List<int> numbers)
    {
        int expected = numbers.Count(ComputeOperations.IsPrime);
        Console.WriteLine($"Очікувана кількість простих чисел (послідовний LINQ): {expected}");
        Console.WriteLine();

        DemoLostUpdates(numbers, expected);
        Console.WriteLine();
        DemoUnsafeList(numbers, expected);
        Console.WriteLine();
        DemoFixes(numbers, expected);
    }

    private static void DemoLostUpdates(List<int> numbers, int expected)
    {
        Console.WriteLine("--- Проблема 1: спільний лічильник без синхронізації (counter++) ---");

        for (int attempt = 1; attempt <= Attempts; attempt++)
        {
            int counter = 0;

            numbers
                .AsParallel()
                .Where(ComputeOperations.IsPrime)
                .ForAll(_ => counter++);

            PrintAttempt(attempt, counter, expected);
        }
    }

    private static void DemoUnsafeList(List<int> numbers, int expected)
    {
        Console.WriteLine("--- Проблема 2: List<int>.Add з кількох потоків ---");

        for (int attempt = 1; attempt <= Attempts; attempt++)
        {
            var shared = new List<int>();

            try
            {
                numbers
                    .AsParallel()
                    .Where(ComputeOperations.IsPrime)
                    .ForAll(n => shared.Add(n));

                PrintAttempt(attempt, shared.Count, expected);
            }
            catch (AggregateException ex)
            {
                Exception inner = ex.InnerException ?? ex;
                Console.WriteLine($"  Спроба {attempt}: виняток {inner.GetType().Name}: {inner.Message}");
            }
        }
    }

    private static void DemoFixes(List<int> numbers, int expected)
    {
        Console.WriteLine("--- Виправлення ---");

        var sw = Stopwatch.StartNew();
        int lockCounter = 0;
        object sync = new object();
        numbers
            .AsParallel()
            .Where(ComputeOperations.IsPrime)
            .ForAll(_ =>
            {
                lock (sync)
                {
                    lockCounter++;
                }
            });
        sw.Stop();
        PrintFix("lock", lockCounter, expected, sw.Elapsed.TotalMilliseconds);

        sw.Restart();
        int interlockedCounter = 0;
        numbers
            .AsParallel()
            .Where(ComputeOperations.IsPrime)
            .ForAll(_ => Interlocked.Increment(ref interlockedCounter));
        sw.Stop();
        PrintFix("Interlocked.Increment", interlockedCounter, expected, sw.Elapsed.TotalMilliseconds);

        sw.Restart();
        var bag = new ConcurrentBag<int>();
        numbers
            .AsParallel()
            .Where(ComputeOperations.IsPrime)
            .ForAll(n => bag.Add(n));
        sw.Stop();
        PrintFix("ConcurrentBag.Add", bag.Count, expected, sw.Elapsed.TotalMilliseconds);

        sw.Restart();
        int withoutSideEffects = numbers
            .AsParallel()
            .Count(ComputeOperations.IsPrime);
        sw.Stop();
        PrintFix("без побічних ефектів (Count)", withoutSideEffects, expected, sw.Elapsed.TotalMilliseconds);
    }

    private static void PrintAttempt(int attempt, int actual, int expected)
    {
        string verdict = actual == expected ? "збігається" : $"ПОМИЛКА, втрачено {expected - actual}";
        Console.WriteLine($"  Спроба {attempt}: отримано {actual}, очікувалось {expected} ({verdict})");
    }

    private static void PrintFix(string name, int actual, int expected, double ms)
    {
        string verdict = actual == expected ? "коректно" : "НЕКОРЕКТНО";
        Console.WriteLine($"  {name}: {actual} (очікувалось {expected}), {verdict}, {ms:F1} мс");
    }
}