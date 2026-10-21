using System.Security.Cryptography;
using System.Text;

class FileMetadata
{
    public string FileName { get; set; } = "";
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = "";

    public override string ToString()
    {
        return $"{FileName}, {Size} байт, змінено {LastModified:dd.MM.yyyy HH:mm}, hash {Hash}";
    }
}

class FileMetadataCache
{
    private Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

    public int Count
    {
        get { return _cache.Count; }
    }

    public void Add(FileMetadata metadata)
    {
        _cache[metadata.FileName] = metadata;
    }

    public FileMetadata? Get(string fileName)
    {
        if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
        {
            return metadata;
        }
        return null;
    }

    public bool Remove(string fileName)
    {
        return _cache.Remove(fileName);
    }

    public bool Contains(string fileName)
    {
        return _cache.ContainsKey(fileName);
    }

    public static FileMetadata FromText(string fileName, string content, DateTime lastModified)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        string hash = Convert.ToHexString(MD5.HashData(bytes)).Substring(0, 8);
        return new FileMetadata { FileName = fileName, Size = bytes.Length, LastModified = lastModified, Hash = hash };
    }
}

class TagManager
{
    private Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.ContainsKey(articleId))
        {
            _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        bool added = _articleTags[articleId].Add(tag);
        if (!added)
        {
            Console.WriteLine($"  Тег '{tag}' вже є у статті {articleId}, дублікат не додано");
        }
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags.Remove(tag);
        }
    }

    public HashSet<string> GetTags(string articleId)
    {
        if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
        }
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> GetAllUniqueTags()
    {
        HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (HashSet<string> tags in _articleTags.Values)
        {
            result.UnionWith(tags);
        }
        return result;
    }

    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        HashSet<string> result = GetTags(articleId1);
        result.IntersectWith(GetTags(articleId2));
        return result;
    }

    public HashSet<string> FindUniqueTags(string articleId1, string articleId2)
    {
        HashSet<string> result = GetTags(articleId1);
        result.ExceptWith(GetTags(articleId2));
        return result;
    }
}

class SearchIndex
{
    private Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

    public int WordCount
    {
        get { return _invertedIndex.Count; }
    }

    public void AddDocument(int docId, string content)
    {
        char[] separators = { ' ', ',', '.', '!', '?', ':', ';', '-', '\n' };
        string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in words)
        {
            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? docs))
            {
                docs = new HashSet<int>();
                _invertedIndex[word] = docs;
            }
            docs.Add(docId);
        }
    }

    public HashSet<int> Search(string word)
    {
        if (_invertedIndex.TryGetValue(word, out HashSet<int>? docs))
        {
            return new HashSet<int>(docs);
        }
        return new HashSet<int>();
    }

    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
        {
            return new HashSet<int>();
        }

        HashSet<int> result = Search(words[0]);
        for (int i = 1; i < words.Length; i++)
        {
            result.IntersectWith(Search(words[i]));
        }
        return result;
    }
}

class Program
{
    static string Show<T>(HashSet<T> set)
    {
        List<T> list = new List<T>(set);
        list.Sort();
        if (list.Count == 0)
        {
            return "{ }";
        }
        return "{ " + string.Join(", ", list) + " }";
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Сценарій 1: FileMetadataCache (Dictionary) ===");
        FileMetadataCache cache = new FileMetadataCache();
        cache.Add(FileMetadataCache.FromText("report.docx", "Звіт з самостійної роботи", new DateTime(2026, 10, 20, 14, 30, 0)));
        cache.Add(FileMetadataCache.FromText("Program.cs", "class Program { static void Main() { } }", new DateTime(2026, 10, 21, 9, 15, 0)));
        cache.Add(FileMetadataCache.FromText("photo.png", "PNG data imitation", new DateTime(2026, 9, 1, 18, 0, 0)));

        Console.WriteLine($"У кеші файлів: {cache.Count}");
        Console.WriteLine($"Get(\"program.cs\") (інший регістр): {cache.Get("program.cs")}");
        Console.WriteLine($"Contains(\"photo.png\") = {cache.Contains("photo.png")}");
        Console.WriteLine($"Remove(\"photo.png\") = {cache.Remove("photo.png")}");
        Console.WriteLine($"Contains(\"photo.png\") = {cache.Contains("photo.png")}");
        Console.WriteLine($"Get(\"missing.txt\") = {(cache.Get("missing.txt") == null ? "null" : "знайдено")}");
        Console.WriteLine();

        Console.WriteLine("=== Сценарій 2: TagManager (HashSet) ===");
        TagManager tags = new TagManager();
        tags.AddTag("post1", "csharp");
        tags.AddTag("post1", "dotnet");
        tags.AddTag("post1", "oop");
        tags.AddTag("post1", "CSharp");
        tags.AddTag("post2", "csharp");
        tags.AddTag("post2", "collections");
        tags.AddTag("post2", "oop");
        tags.AddTag("post3", "python");
        tags.AddTag("post3", "temp");
        tags.RemoveTag("post3", "temp");

        Console.WriteLine($"Теги post1: {Show(tags.GetTags("post1"))}");
        Console.WriteLine($"Теги post2: {Show(tags.GetTags("post2"))}");
        Console.WriteLine($"Теги post3 (після RemoveTag \"temp\"): {Show(tags.GetTags("post3"))}");
        Console.WriteLine($"Усі унікальні теги (UnionWith): {Show(tags.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні теги post1 і post2 (IntersectWith): {Show(tags.FindCommonTags("post1", "post2"))}");
        Console.WriteLine($"Є в post1, але немає в post2 (ExceptWith): {Show(tags.FindUniqueTags("post1", "post2"))}");
        Console.WriteLine();

        Console.WriteLine("=== Сценарій 3: SearchIndex (Dictionary + HashSet) ===");
        SearchIndex index = new SearchIndex();
        index.AddDocument(1, "Dictionary зберігає пари ключ-значення");
        index.AddDocument(2, "HashSet зберігає тільки унікальні значення");
        index.AddDocument(3, "Dictionary і HashSet працюють через хеш-таблицю");
        index.AddDocument(4, "List зберігає елементи по порядку, значення можуть повторюватись");

        Console.WriteLine($"Слів в індексі: {index.WordCount}");
        Console.WriteLine($"Search(\"зберігає\") -> документи {Show(index.Search("зберігає"))}");
        Console.WriteLine($"Search(\"dictionary\") -> документи {Show(index.Search("dictionary"))}");
        Console.WriteLine($"SearchMultiple(\"зберігає\", \"значення\") -> документи {Show(index.SearchMultiple("зберігає", "значення"))}");
        Console.WriteLine($"SearchMultiple(\"Dictionary\", \"HashSet\") -> документи {Show(index.SearchMultiple("Dictionary", "HashSet"))}");
        Console.WriteLine($"Search(\"java\") -> документи {Show(index.Search("java"))}");
    }
}
