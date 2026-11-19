using System.Diagnostics;

namespace InvertedIndexDemo;

public record SearchComparison(
    int DocumentCount,
    string Word,
    int LinearFound,
    int IndexFound,
    double LinearAvgMicros,
    double IndexAvgMicros,
    double BuildMicros)
{
    public double Speedup => IndexAvgMicros > 0 ? LinearAvgMicros / IndexAvgMicros : 0;

    public double BreakEvenSearches => LinearAvgMicros > IndexAvgMicros
        ? BuildMicros / (LinearAvgMicros - IndexAvgMicros)
        : double.PositiveInfinity;
}

public class PerformanceTest
{
    public static SearchComparison CompareSearchMethods(
        List<Document> documents,
        string searchWord,
        int linearRepeats = 20,
        int indexRepeats = 10000)
    {
        string normalizedWord = TextProcessor.Normalize(searchWord);
        var sw = new Stopwatch();

        sw.Start();
        var linearResults = LinearSearch(documents, normalizedWord);
        sw.Stop();
        Console.WriteLine($"Linear search: {sw.ElapsedTicks} ticks");

        sw.Restart();
        for (int i = 0; i < linearRepeats; i++)
        {
            LinearSearch(documents, normalizedWord);
        }
        sw.Stop();
        double linearAvgMicros = sw.Elapsed.TotalMilliseconds * 1000 / linearRepeats;

        sw.Restart();
        var index = new InvertedIndex();
        foreach (var doc in documents)
        {
            index.AddDocument(doc);
        }
        sw.Stop();
        double buildMicros = sw.Elapsed.TotalMilliseconds * 1000;

        sw.Restart();
        var indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        Console.WriteLine($"Inverted Index: {sw.ElapsedTicks} ticks");

        sw.Restart();
        for (int i = 0; i < indexRepeats; i++)
        {
            index.SearchSingleWord(searchWord);
        }
        sw.Stop();
        double indexAvgMicros = sw.Elapsed.TotalMilliseconds * 1000 / indexRepeats;

        Console.WriteLine($"Linear found: {linearResults.Count}, Index found: {indexResults.Count}");

        if (linearResults.Count != indexResults.Count)
        {
            Console.WriteLine("УВАГА: кількість знайдених документів відрізняється, порівняння некоректне.");
        }

        return new SearchComparison(
            documents.Count,
            searchWord,
            linearResults.Count,
            indexResults.Count,
            linearAvgMicros,
            indexAvgMicros,
            buildMicros);
    }

    private static List<Document> LinearSearch(List<Document> documents, string normalizedWord)
    {
        return documents
            .Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord))
            .ToList();
    }
}