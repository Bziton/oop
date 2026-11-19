using System.Globalization;
using System.Text;

namespace InvertedIndexDemo;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var documents = SampleDocuments.GetBaseDocuments();
        var index = new InvertedIndex();

        foreach (var doc in documents)
        {
            index.AddDocument(doc);
        }

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine();

        DemoSearch(index);
        DemoTextProcessing();
        DemoPerformance();

        if (args.Contains("interactive"))
        {
            RunInteractive(index);
        }
    }

    private static void DemoSearch(InvertedIndex index)
    {
        Console.WriteLine("--- Пошук ---");

        PrintResults("AND: programming language", index.SearchMultipleWords("programming", "language"));
        PrintResults("AND: c# async", index.SearchMultipleWords("c#", "async"));
        PrintResults("OR: java python", index.SearchAnyWord("java", "python"));
        PrintResults("Одне слово: popular", index.SearchSingleWord("popular"));
        PrintResults("AND: popular linq", index.SearchMultipleWords("popular", "linq"));
        Console.WriteLine();
    }

    private static void PrintResults(string title, List<Document> results)
    {
        Console.WriteLine($"{title} -> {results.Count} документів");
        foreach (var doc in results)
        {
            Console.WriteLine($"  {doc}");
        }
    }

    private static void DemoTextProcessing()
    {
        Console.WriteLine("--- Обробка тексту ---");
        string text = "The C# language is a modern programming language";
        var tokens = TextProcessor.Tokenize(text).ToList();
        var withoutStopWords = TextProcessor.RemoveStopWords(tokens).ToList();

        Console.WriteLine($"Токени: {string.Join(", ", tokens)}");
        Console.WriteLine($"Без стоп-слів: {string.Join(", ", withoutStopWords)}");
        Console.WriteLine();
    }

    private static void DemoPerformance()
    {
        Console.WriteLine("--- Порівняння продуктивності: лінійний пошук і Inverted Index ---");
        const string word = "programming";

        Console.WriteLine("Прогрів JIT (результат не враховується):");
        PerformanceTest.CompareSearchMethods(SampleDocuments.Generate(10), word);
        Console.WriteLine();

        var results = new List<SearchComparison>();

        foreach (int count in new[] { 10, 100, 1000, 10000 })
        {
            var documents = SampleDocuments.Generate(count);
            Console.WriteLine($"Документів: {count}, слово \"{word}\"");
            results.Add(PerformanceTest.CompareSearchMethods(documents, word));
            Console.WriteLine();
        }

        Console.WriteLine("Документів | Знайдено | Лінійний, мкс | Індекс, мкс | Побудова, мкс | Прискорення | Окупається після");
        foreach (var r in results)
        {
            Console.WriteLine(
                $"{r.DocumentCount,10} | {r.IndexFound,8} | {r.LinearAvgMicros,13:F2} | {r.IndexAvgMicros,11:F3} | {r.BuildMicros,13:F1} | {r.Speedup,10:F1}x | {r.BreakEvenSearches,8:F1} пошуків");
        }
    }

    private static void RunInteractive(InvertedIndex index)
    {
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query == null || query == "exit")
            {
                break;
            }

            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("\n--- Search Results (AND) ---");
            var results = index.SearchMultipleWords(words);

            foreach (var doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
            }

            Console.WriteLine($"\nFound {results.Count} documents\n");
        }
    }
}