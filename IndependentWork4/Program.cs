using System;
using System.Collections.Generic;
using System.Text;

class LibraryItem
{
    private string _title;
    private int _publicationYear;
    private string _uniqueId;

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int PublicationYear
    {
        get { return _publicationYear; }
        set { _publicationYear = value; }
    }

    public string UniqueId
    {
        get { return _uniqueId; }
    }

    public LibraryItem(string title, int publicationYear, string uniqueId)
    {
        _uniqueId = uniqueId;
        _title = title;
        _publicationYear = publicationYear;
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"[{UniqueId}] Назва: {Title}, рік видання: {PublicationYear}");
    }
}

class Book : LibraryItem
{
    private string _author;

    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    public Book(string title, int publicationYear, string author, string uniqueId) : base(title, publicationYear, uniqueId)
    {
        _author = author;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"    Автор: {Author}");
    }
}

class Journal : LibraryItem
{
    private int _issueNumber;

    public int IssueNumber
    {
        get { return _issueNumber; }
        set { _issueNumber = value; }
    }

    public Journal(string title, int publicationYear, int issueNumber, string uniqueId) : base(title, publicationYear, uniqueId)
    {
        _issueNumber = issueNumber;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"    Номер випуску: {IssueNumber}");
    }
}

class AudioBook : Book
{
    private string _narrator;

    public string Narrator
    {
        get { return _narrator; }
        set { _narrator = value; }
    }

    public AudioBook(string title, int publicationYear, string author, string narrator, string uniqueId) : base(title, publicationYear, author, uniqueId)
    {
        _narrator = narrator;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"    Оповідач: {Narrator}");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Book book = new Book("Тіні забутих предків", 1911, "Михайло Коцюбинський", "B-001");
        Journal journal = new Journal("Наука і життя", 2024, 7, "J-014");
        AudioBook audioBook = new AudioBook("Кайдашева сім'я", 1879, "Іван Нечуй-Левицький", "Остап Ступка", "A-003");

        Console.WriteLine("=== Book ===");
        book.DisplayDetails();
        Console.WriteLine();

        Console.WriteLine("=== Journal ===");
        journal.DisplayDetails();
        Console.WriteLine();

        Console.WriteLine("=== AudioBook ===");
        audioBook.DisplayDetails();
        Console.WriteLine();

        Console.WriteLine("=== Поліморфний виклик через List<LibraryItem> ===");
        List<LibraryItem> items = new List<LibraryItem> { book, journal, audioBook };
        foreach (LibraryItem item in items)
        {
            Console.WriteLine($"{item.GetType().Name}:");
            item.DisplayDetails();
        }
    }
}
