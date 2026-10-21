# Самостійна робота №5

> **Тема:** Приклади поліморфізму в .NET.

---

## Зміст

- [Хід роботи](#хід-роботи)
- [Сценарій 1: System.Object](#сценарій-1-systemobject)
- [Сценарій 2: IEnumerable](#сценарій-2-ienumerable)
- [Сценарій 3: Stream](#сценарій-3-stream)
- [Висновки](#висновки)
- [Результати](#результати)

---

## Хід роботи

```bash
dotnet new console -o IndependentWork5
```

Дослідив три сценарії поліморфізму, які є в самому .NET: через базовий клас `Object`, через інтерфейс `IEnumerable<T>` і через абстрактний клас `Stream`.

---

## Сценарій 1: System.Object

Створив класи `Product`, `Customer`, `Order` і в кожному перевизначив `ToString()`. Для порівняння додав клас `Warehouse` без перевизначення.

```csharp
class Customer
{
    private string _fullName;
    private string _email;

    public override string ToString()
    {
        return $"Клієнт: {FullName} ({Email})";
    }
}
```

Усі об'єкти поклав у `List<object>` разом з числом і рядком і вивів у циклі:

```csharp
List<object> objects = new List<object> { laptop, customer, order, new Warehouse("Рівне"), 42, "просто рядок" };
foreach (object obj in objects)
{
    Console.WriteLine($"{obj.GetType().Name,-10} -> {obj}");
}
```

**Як проявляється поліморфізм:**

- Змінна `obj` має тип `object`, але `ToString()` віртуальний, тому викликається версія фактичного типу об'єкта: для `Product` — `Product.ToString()`, для `Order` — `Order.ToString()` і т.д.
- Для `Warehouse` викликається реалізація з `System.Object`, яка повертає просто назву типу (`Warehouse`).
- `int` і `string` теж перевизначають `ToString()`, тому виводиться `42` і текст рядка.

**Переваги:** `Console.WriteLine` та інтерполяція рядків працюють з будь-яким об'єктом, не знаючи його типу. Кожен клас сам вирішує, як себе показати.

---

## Сценарій 2: IEnumerable

Створив клас `MyCustomCollection`, який всередині зберігає `List<string>` і реалізує `IEnumerable<string>`:

```csharp
class MyCustomCollection : IEnumerable<string>
{
    private List<string> _items = new List<string>();

    public void Add(string item)
    {
        _items.Add(item);
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (string item in _items)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

Для `foreach` достатньо методу `GetEnumerator()`. `yield return` сам створює енумератор, тому не треба писати окремий клас з `MoveNext()` і `Current`. Неузагальнений `IEnumerable.GetEnumerator()` реалізований явно, бо `IEnumerable<T>` успадковується від `IEnumerable`.

Також написав метод, який приймає `IEnumerable<string>`, і передав йому три різні колекції:

```csharp
static void PrintAll(IEnumerable<string> items)
{
    foreach (string item in items)
    {
        Console.WriteLine($" - {item}");
    }
}

PrintAll(collection);
PrintAll(new List<string> { "List 1", "List 2" });
PrintAll(new string[] { "Array 1", "Array 2" });
```

**Як проявляється поліморфізм:** `foreach` і `PrintAll` працюють через інтерфейс і не знають, що всередині — мій клас, `List<string>` чи масив. Кожен тип по-своєму реалізує `GetEnumerator()`.

**Переваги:** один і той самий код перебору для будь-якої колекції. Власна колекція одразу працює з `foreach` (і з LINQ), а внутрішній `List` залишається прихованим — зовні можна тільки додавати і перебирати.

---

## Сценарій 3: Stream

Написав метод, який приймає абстрактний `Stream` і читає з нього перші 16 байт:

```csharp
static void ProcessStream(Stream stream)
{
    Console.WriteLine($"Тип потоку: {stream.GetType().Name}, довжина: {stream.Length} байт");

    stream.Position = 0;
    byte[] buffer = new byte[16];
    int read = stream.Read(buffer, 0, buffer.Length);

    Console.WriteLine($"Прочитано {read} байт: {Encoding.UTF8.GetString(buffer, 0, read)}");
}
```

Тестовий файл створюється прямо в програмі, тому проєкт не залежить від зовнішніх файлів:

```csharp
string filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
File.WriteAllText(filePath, "Hello from FileStream!");

using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
{
    ProcessStream(fileStream);
}

using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Data in MemoryStream")))
{
    ProcessStream(memoryStream);
}
```

**Як проявляється поліморфізм:** `Read`, `Length`, `Position` — абстрактні члени класу `Stream`. `FileStream` реалізує їх через читання з диска, `MemoryStream` — через масив байтів у пам'яті. Метод `ProcessStream` викликає одні й ті самі методи, а виконується реалізація конкретного потоку.

**Переваги:** код обробки пишеться один раз і працює з будь-яким потоком (файл, пам'ять, мережа, архів). Щоб перевірити метод, не обов'язково створювати файл — можна передати `MemoryStream`.

---

## Висновки

| Сценарій | Механізм | Спільний код | Різні реалізації |
|---|---|---|---|
| `Object` | віртуальний метод `ToString()` | `Console.WriteLine(obj)` | `Product`, `Customer`, `Order`, `int`, `string` |
| `IEnumerable<T>` | інтерфейс | `foreach`, `PrintAll` | `MyCustomCollection`, `List<string>`, `string[]` |
| `Stream` | абстрактний клас | `ProcessStream` | `FileStream`, `MemoryStream` |

У всіх трьох випадках клієнтський код залежить від базового типу (клас, інтерфейс або абстрактний клас), а не від конкретної реалізації. Через це код гнучкий, його можна використати повторно, і нові типи можна додавати без змін у вже написаних методах.

---

## Результати

![alt text](image.png)
