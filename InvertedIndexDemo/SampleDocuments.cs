namespace InvertedIndexDemo;

public static class SampleDocuments
{
    private static readonly string[] Vocabulary =
    {
        "c#", "java", "python", "programming", "language", "modern", "popular", "performance",
        "syntax", "linq", "async", "await", "compiler", "runtime", "memory", "object",
        "oriented", "functional", "paradigm", "library", "framework", "developer", "software", "web",
        "database", "network", "algorithm", "data", "structure", "testing", "debugging", "cloud",
        "mobile", "security", "learning", "portability", "microsoft", "simple", "fast", "powerful"
    };

    public static List<Document> GetBaseDocuments()
    {
        return new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Introduction to C#",
                Content = "C# is a modern object-oriented programming language developed by Microsoft."
            },
            new Document
            {
                Id = 2,
                Title = "Java Basics",
                Content = "Java is a popular programming language known for its portability and performance."
            },
            new Document
            {
                Id = 3,
                Title = "Python Overview",
                Content = "Python is a high-level programming language with simple syntax."
            },
            new Document
            {
                Id = 4,
                Title = "C# Advanced Features",
                Content = "C# supports LINQ, async/await, and many modern programming paradigms."
            },
            new Document
            {
                Id = 5,
                Title = "Comparing Languages",
                Content = "C#, Java, and Python are among the most popular programming languages today."
            }
        };
    }

    public static List<Document> Generate(int count, int seed = 42)
    {
        var rnd = new Random(seed);
        var documents = new List<Document>(count);

        for (int id = 1; id <= count; id++)
        {
            int wordCount = rnd.Next(8, 16);
            var words = new string[wordCount];

            for (int i = 0; i < wordCount; i++)
            {
                words[i] = Vocabulary[rnd.Next(Vocabulary.Length)];
            }

            documents.Add(new Document
            {
                Id = id,
                Title = $"Generated document {id}",
                Content = string.Join(" ", words) + "."
            });
        }

        return documents;
    }
}