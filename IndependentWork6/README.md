# Самостійна робота №6

> **Тема:** Інтерфейс vs абстрактний клас - практичні критерії.

---

## Зміст

- [Хід роботи](#хід-роботи)
- [Сценарій 1: Система сповіщень](#сценарій-1-система-сповіщень)
- [Сценарій 2: Обробка файлів](#сценарій-2-обробка-файлів)
- [Загальні висновки](#загальні-висновки)
- [Результати](#результати)

---

## Хід роботи

```bash
dotnet new console -o IndependentWork6
```

Перед написанням коду порівняв обидва сценарії з таблицею "абстрактний клас vs інтерфейс":

| Критерій | Сповіщення | Обробка файлів |
|---|---|---|
| Є спільний код у реалізаціях? | ні, кожен канал відправляє по-своєму | так, відкриття і закриття однакові |
| Є спільний стан (поля)? | ні | так, шлях до файлу і вміст |
| Чи може клас мати ще інші ролі? | так (наприклад логер) | ні |
| Відношення між класами | "вміє відправляти" (can-do) | "є обробником файлу" (is-a) |
| **Вибір** | **інтерфейс** | **абстрактний клас** |

---

## Сценарій 1: Система сповіщень

### Обраний підхід: Інтерфейс `INotifier`

### Обґрунтування вибору:

У цьому сценарії краще використовувати інтерфейс `INotifier`, оскільки:

1. **Різні базові типи:** `EmailNotifier`, `SmsNotifier`, `PushNotifier` не мають спільної реалізації — у них різні поля (email, телефон, id пристрою) і різна логіка відправки. Спільний тільки контракт: метод `Send(string message)`. Саме це і описує інтерфейс.
2. **Множинна поведінка:** клас може бути не тільки сповіщувачем. `PushNotifier` реалізує ще й `ILogger`. З абстрактним класом так не вийде, бо в C# можна успадкуватися лише від одного класу.
3. **Слабке зв'язування:** клієнтський код працює з `List<INotifier>` і не залежить від конкретних каналів. Новий канал (Telegram, Viber) додається новим класом без змін існуючого коду.

**Чому абстрактний клас тут менш доречний:** у ньому не було б що писати, крім абстрактного `Send()`, бо спільного коду немає. При цьому він "забрав" би єдиний базовий клас у кожного сповіщувача, і такий клас вже не зміг би успадкуватися від чогось іншого.

### Реалізація:

```csharp
interface INotifier
{
    string ChannelName { get; }
    void Send(string message);
}

interface ILogger
{
    void Log(string text);
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
    ...
}

List<INotifier> notifiers = new List<INotifier>();
notifiers.Add(new EmailNotifier("student@rcit.ukr.education"));
notifiers.Add(new SmsNotifier("+380991234567"));
notifiers.Add(new PushNotifier("phone-01"));

foreach (INotifier notifier in notifiers)
{
    notifier.Send("Завтра пара з ООП переноситься на 10:00");
}
```

Кожен клас має свою логіку: email відправляється повністю, SMS обрізається до 30 символів, push додатково пише в лог. У циклі через `is ILogger` перевіряється, які об'єкти мають ще й роль логера.

---

## Сценарій 2: Обробка файлів

### Обраний підхід: Абстрактний клас `FileProcessor`

### Обґрунтування вибору:

У цьому сценарії краще використовувати абстрактний клас `FileProcessor`, оскільки:

1. **Спільні кроки:** відкриття і закриття файлу однакові для всіх типів. В абстрактному класі вони пишуться один раз, а нащадки реалізують тільки `ProcessContent()`.
2. **Спільний стан:** усім обробникам потрібні шлях до файлу і його вміст. Абстрактний клас може мати поля і конструктор, інтерфейс — ні.
3. **Контроль порядку виконання (шаблонний метод):** публічний метод `Process()` завжди виконує `Open → ProcessContent → Close`. Нащадок не може забути закрити файл або змінити порядок, бо `Open()` і `Close()` приватні.
4. **Відношення is-a:** `XmlFileProcessor` **є** обробником файлу, а не просто "вміє" щось робити.

**Чому інтерфейс тут менш доречний:** в інтерфейсі `IFileProcessor` з методами `Open`, `Process`, `Close` кожен клас мав би сам реалізувати відкриття і закриття, тобто однаковий код копіювався б у 3 класи. І ніщо не гарантувало б, що їх викликають у правильному порядку.

### Реалізація:

```csharp
abstract class FileProcessor
{
    private string _filePath;
    private string _content = "";

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

List<FileProcessor> processors = new List<FileProcessor>();
processors.Add(new TextFileProcessor(txtPath));
processors.Add(new XmlFileProcessor(xmlPath));
processors.Add(new CsvFileProcessor(csvPath));

foreach (FileProcessor processor in processors)
{
    processor.Process();
}
```

Реалізації: `TextFileProcessor` рахує рядки і слова, `XmlFileProcessor` через `XmlDocument` знаходить елементи `<student>`, `CsvFileProcessor` рахує середній бал. Тестові файли створюються в `Main` через `File.WriteAllText`.

---

## Загальні висновки

**Інтерфейс обирати, коли:**

- потрібен тільки контракт ("що вміє робити"), а спільного коду немає;
- реалізації не пов'язані між собою;
- клас має поєднувати кілька ролей (множинна реалізація інтерфейсів);
- важливе слабке зв'язування і заміна реалізацій (принцип DIP — залежати від абстракції).

**Абстрактний клас обирати, коли:**

- у нащадків є спільний код або спільні поля;
- потрібен конструктор для ініціалізації спільного стану;
- треба зафіксувати алгоритм, а нащадкам дати змінити тільки окремі кроки (шаблонний метод);
- класи пов'язані відношенням "є" (is-a).

Поліморфізм в обох прикладах однаковий: колекція базового типу (`List<INotifier>` або `List<FileProcessor>`) і один цикл, який викликає спільний метод, а виконується реалізація конкретного класу. Різниця в тому, що інтерфейс дає тільки контракт, а абстрактний клас ще й частину готової реалізації. Ці підходи можна поєднувати: абстрактний клас може реалізовувати інтерфейс.

---

## Результати

![alt text](image.png)
