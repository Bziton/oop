namespace IndependentWork14;

public class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags =
        new Dictionary<string, HashSet<string>>();

    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId] = tags;
        }

        tags.Add(tag);
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags.Remove(tag);

            if (tags.Count == 0)
            {
                _articleTags.Remove(articleId);
            }
        }
    }

    public HashSet<string> GetTags(string articleId)
    {
        return _articleTags.TryGetValue(articleId, out HashSet<string>? tags)
            ? new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> GetAllUniqueTags()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

    public HashSet<string> FindUniqueTags(string articleId, string otherArticleId)
    {
        HashSet<string> result = GetTags(articleId);
        result.ExceptWith(GetTags(otherArticleId));
        return result;
    }
}