using System.Text.RegularExpressions;

namespace InvertedIndexDemo;

public static class TextProcessor
{
    private static readonly Regex WordPattern = new Regex(@"\w+(?:#|\+\+)?", RegexOptions.Compiled);

    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

    public static IEnumerable<string> Tokenize(string text)
    {
        return WordPattern
            .Matches(text.ToLowerInvariant())
            .Select(m => m.Value);
    }

    public static string Normalize(string word)
    {
        return Tokenize(word).FirstOrDefault() ?? string.Empty;
    }

    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }
}