namespace IndependentWork14;

public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;
}

public class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _cache =
        new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

    public int Count => _cache.Count;

    public void Add(FileMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _cache[metadata.FileName] = metadata;
    }

    public FileMetadata? Get(string fileName)
    {
        return _cache.TryGetValue(fileName, out FileMetadata? metadata) ? metadata : null;
    }

    public bool Remove(string fileName)
    {
        return _cache.Remove(fileName);
    }

    public bool Contains(string fileName)
    {
        return _cache.ContainsKey(fileName);
    }
}