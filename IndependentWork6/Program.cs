using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

interface INotifier
{
    string ChannelName { get; }
    void Send(string message);
}

interface ILogger
{
    void Log(string text);
}

class EmailNotifier : INotifier
{
    private string _email;

    public string ChannelName
    {
        get { return "Email"; }
    }

    public EmailNotifier(string email)
    {
        _email = email;
    }

    public void Send(string message)
    {
        Console.WriteLine($"[Email] Лист на {_email}: \"{message}\"");
    }
}

class SmsNotifier : INotifier
{
    private string _phone;

    public string ChannelName
    {
        get { return "SMS"; }
    }

    public SmsNotifier(string phone)
    {
        _phone = phone;
    }

    public void Send(string message)
    {
        string text = message.Length > 30 ? message.Substring(0, 30) + "..." : message;
        Console.WriteLine($"[SMS] На номер {_phone}: \"{text}\"");
    }
}

class PushNotifier : INotifier, ILogger
{
    private string _deviceId;
    private int _sentCount;

    public string ChannelName
    {
        get { return "Push"; }
    }

    public PushNotifier(string deviceId)
    {
        _deviceId = deviceId;
    }

    public void Send(string message)
    {
        _sentCount++;
        Console.WriteLine($"[Push] Пристрій {_deviceId}: \"{message}\"");
        Log($"відправлено push №{_sentCount}");
    }

    public void Log(string text)
    {
        Console.WriteLine($"    (лог) {text}");
    }
}

abstract class FileProcessor
{
    private string _filePath;
    private string _content = "";

    public string FilePath
    {
        get { return _filePath; }
    }

    protected string Content
    {
        get { return _content; }
    }

    protected FileProcessor(string filePath)
    {
        _filePath = filePath;
    }

    public void Process()
    {
        Open();
        ProcessContent();
        Close();
    }

    private void Open()
    {
        _content = File.ReadAllText(_filePath);
        Console.WriteLine($"Відкрито файл {Path.GetFileName(_filePath)} ({_content.Length} символів)");
    }

    protected abstract void ProcessContent();

    private void Close()
    {
        _content = "";
        Console.WriteLine($"Файл {Path.GetFileName(_filePath)} закрито");
    }
}

class TextFileProcessor : FileProcessor
{
    public TextFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent()
    {
        string[] lines = Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string[] words = Content.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"    Текстовий файл: рядків = {lines.Length}, слів = {words.Length}");
    }
}

class XmlFileProcessor : FileProcessor
{
    public XmlFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent()
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(Content);
        XmlNodeList? students = document.SelectNodes("/group/student");
        Console.WriteLine($"    XML файл: кореневий елемент <{document.DocumentElement?.Name}>, студентів = {students?.Count}");
        if (students != null)
        {
            foreach (XmlNode student in students)
            {
                Console.WriteLine($"    - {student.InnerText}");
            }
        }
    }
}

class CsvFileProcessor : FileProcessor
{
    public CsvFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent()
    {
        string[] lines = Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        double sum = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            sum += Convert.ToDouble(parts[1].Trim());
        }
        Console.WriteLine($"    CSV файл: записів = {lines.Length - 1}, середній бал = {sum / (lines.Length - 1):F1}");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Сценарій 1: Система сповіщень (інтерфейс INotifier) ===");
        List<INotifier> notifiers = new List<INotifier>();
        notifiers.Add(new EmailNotifier("student@rcit.ukr.education"));
        notifiers.Add(new SmsNotifier("+380991234567"));
        notifiers.Add(new PushNotifier("phone-01"));

        foreach (INotifier notifier in notifiers)
        {
            notifier.Send("Завтра пара з ООП переноситься на 10:00");
        }

        Console.WriteLine();
        Console.WriteLine("Об'єкти, які крім INotifier реалізують ще й ILogger:");
        foreach (INotifier notifier in notifiers)
        {
            if (notifier is ILogger logger)
            {
                logger.Log($"{notifier.ChannelName} підтримує логування");
            }
        }

        Console.WriteLine();
        Console.WriteLine("=== Сценарій 2: Обробка файлів (абстрактний клас FileProcessor) ===");
        string folder = AppContext.BaseDirectory;
        string txtPath = Path.Combine(folder, "notes.txt");
        string xmlPath = Path.Combine(folder, "group.xml");
        string csvPath = Path.Combine(folder, "grades.csv");

        File.WriteAllText(txtPath, "Інтерфейс описує що вміє об'єкт\nАбстрактний клас дає спільну реалізацію\n");
        File.WriteAllText(xmlPath, "<group name=\"ІПЗ-3/2\"><student>Петренко</student><student>Коваленко</student><student>Шевчук</student></group>");
        File.WriteAllText(csvPath, "Студент;Бал\nПетренко;92\nКоваленко;88\nШевчук;79\n");

        List<FileProcessor> processors = new List<FileProcessor>();
        processors.Add(new TextFileProcessor(txtPath));
        processors.Add(new XmlFileProcessor(xmlPath));
        processors.Add(new CsvFileProcessor(csvPath));

        foreach (FileProcessor processor in processors)
        {
            processor.Process();
            Console.WriteLine();
        }
    }
}
