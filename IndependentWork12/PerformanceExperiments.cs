using System.Diagnostics;

namespace IndependentWork12;

public record ExperimentResult(
    string Operation,
    int Size,
    double LinqMs,
    double PlinqMs,
    int LinqCount,
    int PlinqCount,
    bool Match)
{
    public double Speedup => PlinqMs > 0 ? LinqMs / PlinqMs : 0;
}

public static class PerformanceExperiments
{
    private const int Runs = 3;

    public static void WarmUp(List<int> data)
    {
        HeavyLinq(data);
        HeavyPlinq(data);
        LightLinq(data);
        LightPlinq(data);
    }

    public static ExperimentResult RunHeavy(List<int> data)
    {
        var (linqResult, linqMs) = Measure(() => HeavyLinq(data));
        var (plinqResult, plinqMs) = Measure(() => HeavyPlinq(data));

        double linqSum = linqResult.Sum();
        double plinqSum = plinqResult.Sum();
        bool match = linqResult.Count == plinqResult.Count
            && Math.Abs(linqSum - plinqSum) <= 1e-9 * Math.Max(1.0, linqSum);

        return new ExperimentResult(
            "Важка (IsPrime + Sqrt)", data.Count, linqMs, plinqMs, linqResult.Count, plinqResult.Count, match);
    }

    public static ExperimentResult RunLight(List<int> data)
    {
        var (linqResult, linqMs) = Measure(() => LightLinq(data));
        var (plinqResult, plinqMs) = Measure(() => LightPlinq(data));

        bool match = linqResult.Count == plinqResult.Count
            && linqResult.Sum() == plinqResult.Sum();

        return new ExperimentResult(
            "Легка (парні, x * 2)", data.Count, linqMs, plinqMs, linqResult.Count, plinqResult.Count, match);
    }

    private static List<double> HeavyLinq(List<int> data)
    {
        return data
            .Where(ComputeOperations.IsPrime)
            .Select(n => Math.Sqrt(n))
            .ToList();
    }

    private static List<double> HeavyPlinq(List<int> data)
    {
        return data
            .AsParallel()
            .Where(ComputeOperations.IsPrime)
            .Select(n => Math.Sqrt(n))
            .ToList();
    }

    private static List<long> LightLinq(List<int> data)
    {
        return data
            .Where(n => n % 2 == 0)
            .Select(n => n * 2L)
            .ToList();
    }

    private static List<long> LightPlinq(List<int> data)
    {
        return data
            .AsParallel()
            .Where(n => n % 2 == 0)
            .Select(n => n * 2L)
            .ToList();
    }

    private static (T Result, double AverageMs) Measure<T>(Func<T> query)
    {
        T result = default!;
        double totalMs = 0;

        for (int i = 0; i < Runs; i++)
        {
            var sw = Stopwatch.StartNew();
            result = query();
            sw.Stop();
            totalMs += sw.Elapsed.TotalMilliseconds;
        }

        return (result, totalMs / Runs);
    }
}