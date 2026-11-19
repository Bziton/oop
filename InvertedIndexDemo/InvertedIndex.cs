namespace InvertedIndexDemo;

public class InvertedIndex
{
    private readonly Dictionary<string, HashSet<int>> _index;
    private readonly Dictionary<int, Document> _documents;

    public InvertedIndex()
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
    }

    public int DocumentCount => _documents.Count;

    public int UniqueWords => _index.Keys.Count;

    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (!_documents.TryAdd(doc.Id, doc))
        {
            throw new ArgumentException($"Документ з Id {doc.Id} уже додано до індексу.", nameof(doc));
        }

        foreach (string word in TextProcessor.Tokenize(doc.Content))
        {
            if (!_index.TryGetValue(word, out HashSet<int>? ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }

            ids.Add(doc.Id);
        }
    }

    public List<Document> SearchSingleWord(string word)
    {
        string normalized = TextProcessor.Normalize(word);

        if (_index.TryGetValue(normalized, out HashSet<int>? ids))
        {
            return ToDocuments(ids);
        }

        return new List<Document>();
    }

    public List<Document> SearchMultipleWords(params string[] words)
    {
        if (words == null || words.Length == 0)
        {
            return new List<Document>();
        }

        HashSet<int>? result = null;

        foreach (string word in words)
        {
            string normalized = TextProcessor.Normalize(word);

            if (!_index.TryGetValue(normalized, out HashSet<int>? ids))
            {
                return new List<Document>();
            }

            if (result == null)
            {
                result = new HashSet<int>(ids);
            }
            else
            {
                result.IntersectWith(ids);

                if (result.Count == 0)
                {
                    return new List<Document>();
                }
            }
        }

        return result == null ? new List<Document>() : ToDocuments(result);
    }

    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();

        if (words == null)
        {
            return new List<Document>();
        }

        foreach (string word in words)
        {
            string normalized = TextProcessor.Normalize(word);

            if (_index.TryGetValue(normalized, out HashSet<int>? ids))
            {
                result.UnionWith(ids);
            }
        }

        return ToDocuments(result);
    }

    private List<Document> ToDocuments(IEnumerable<int> ids)
    {
        return ids
            .OrderBy(id => id)
            .Select(id => _documents[id])
            .ToList();
    }
}