using System.Globalization;
using System.Text;

namespace IndependentWork14;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        RunFileMetadataCache();
        Console.WriteLine();
        RunTagManager();
        Console.WriteLine();
        RunSearchIndex();
    }

    private static void RunFileMetadataCache()
    {
        Console.WriteLine("--- Сценарій 1: FileMetadataCache (Dictionary) ---");
        var cache = new FileMetadataCache();

        cache.Add(new FileMetadata
        {
            FileName = "report.pdf",
            Size = 245760,
            LastModified = new DateTime(2026, 11, 10, 14, 30, 0),
            Hash = "a1b2c3"
        });
        cache.Add(new FileMetadata
        {
            FileName = "photo.png",
            Size = 1048576,
            LastModified = new DateTime(2026, 11, 12, 9, 15, 0),
            Hash = "d4e5f6"
        });
        cache.Add(new FileMetadata
        {
            FileName = "notes.txt",
            Size = 2048,
            LastModified = new DateTime(2026, 11, 15, 18, 45, 0),
            Hash = "0a9b8c"
        });

        Console.WriteLine($"Кількість файлів у кеші: {cache.Count}");

        FileMetadata? found = cache.Get("REPORT.PDF");
        if (found != null)
        {
            Console.WriteLine($"Get(\"REPORT.PDF\"): {found.FileName}, {found.Size} байт, {found.LastModified:dd.MM.yyyy HH:mm}, хеш {found.Hash}");
        }

        Console.WriteLine($"Contains(\"notes.txt\") = {cache.Contains("notes.txt")}");
        Console.WriteLine($"Remove(\"photo.png\") = {cache.Remove("photo.png")}");
        Console.WriteLine($"Contains(\"photo.png\") = {cache.Contains("photo.png")}");
        Console.WriteLine($"Get(\"photo.png\") == null: {cache.Get("photo.png") == null}");
        Console.WriteLine($"Кількість файлів у кеші: {cache.Count}");
    }

    private static void RunTagManager()
    {
        Console.WriteLine("--- Сценарій 2: TagManager (Dictionary + HashSet) ---");
        var manager = new TagManager();

        manager.AddTag("article1", "csharp");
        manager.AddTag("article1", "linq");
        manager.AddTag("article1", "collections");

        manager.AddTag("article2", "csharp");
        manager.AddTag("article2", "generics");
        manager.AddTag("article2", "collections");
        manager.AddTag("article2", "CSHARP");

        manager.AddTag("article3", "linq");
        manager.AddTag("article3", "async");

        Console.WriteLine($"Теги article2 (дубль CSHARP проігноровано): {Join(manager.GetTags("article2"))}");
        Console.WriteLine($"Усі унікальні теги: {Join(manager.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні теги article1 і article2: {Join(manager.FindCommonTags("article1", "article2"))}");
        Console.WriteLine($"Теги article1, яких немає в article2: {Join(manager.FindUniqueTags("article1", "article2"))}");

        manager.RemoveTag("article3", "async");
        Console.WriteLine($"Усі унікальні теги після видалення async: {Join(manager.GetAllUniqueTags())}");
    }

    private static void RunSearchIndex()
    {
        Console.WriteLine("--- Сценарій 3: SearchIndex (Dictionary<string, HashSet<int>>) ---");
        var index = new SearchIndex();

        index.AddDocument(1, "LINQ makes collections easy to query.");
        index.AddDocument(2, "Dictionary and HashSet are fast collections.");
        index.AddDocument(3, "HashSet stores unique values, Dictionary stores pairs.");

        Console.WriteLine($"Search(\"collections\"): {Join(index.Search("collections"))}");
        Console.WriteLine($"Search(\"hashset\"): {Join(index.Search("hashset"))}");
        Console.WriteLine($"Search(\"python\"): {Join(index.Search("python"))}");
        Console.WriteLine($"SearchMultiple(\"hashset\", \"dictionary\"): {Join(index.SearchMultiple("hashset", "dictionary"))}");
        Console.WriteLine($"SearchMultiple(\"hashset\", \"linq\"): {Join(index.SearchMultiple("hashset", "linq"))}");
    }

    private static string Join(IEnumerable<string> items)
    {
        string text = string.Join(", ", items.OrderBy(i => i, StringComparer.Ordinal));
        return text.Length == 0 ? "немає" : text;
    }

    private static string Join(IEnumerable<int> items)
    {
        string text = string.Join(", ", items.OrderBy(i => i));
        return text.Length == 0 ? "немає" : text;
    }
}