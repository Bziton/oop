# Самостійна робота №7

> **Тема:** Узагальнений кеш Cache<TKey, TValue> із тестами.

---

## Зміст

- [Хід роботи](#хід-роботи)
- [Реалізація кешу](#реалізація-кешу)
- [Юніт-тести](#юніт-тести)
- [Результати](#результати)

---

## Хід роботи

### 1. Створив проєкти

Основний консольний проєкт і проєкт з тестами (xUnit), з тестового проєкту додав посилання на основний:

```bash
dotnet new console -o IndependentWork7
dotnet new xunit -o IndependentWork7.Tests
dotnet add IndependentWork7.Tests/IndependentWork7.Tests.csproj reference IndependentWork7/IndependentWork7.csproj
```

Структура:

```
oop/
├── IndependentWork7/
│   ├── Cache.cs
│   ├── Program.cs
│   └── IndependentWork7.csproj
└── IndependentWork7.Tests/
    ├── CacheTests.cs
    └── IndependentWork7.Tests.csproj
```

Запуск:

```bash
cd IndependentWork7
dotnet run

cd ../IndependentWork7.Tests
dotnet test
```

---

## Реалізація кешу

Клас `Cache<TKey, TValue>` (файл `Cache.cs`) зберігає пари ключ-значення з обмеженою ємністю і політикою FIFO.

- Приватні поля:
  - `_capacity` (int) — максимальна кількість елементів;
  - `_cache` (`Dictionary<TKey, TValue>`) — зберігає значення, пошук за ключем O(1);
  - `_keysOrder` (`Queue<TKey>`) — порядок додавання ключів, першим у черзі завжди найстаріший.
- Конструктор `Cache(int capacity)` — якщо ємність ≤ 0, кидає `ArgumentOutOfRangeException`.
- `Add(key, value)` — якщо ключ вже є, просто оновлює значення. Якщо кеш повний — `Dequeue()` найстарішого ключа і видалення його зі словника, потім додавання нового.
- `Get(key)` — через `TryGetValue`, якщо ключа немає — `KeyNotFoundException` з моїм повідомленням.
- `ContainsKey(key)` — перевірка наявності.
- `Count` — поточна кількість елементів.

```csharp
public void Add(TKey key, TValue value)
{
    if (_cache.ContainsKey(key))
    {
        _cache[key] = value;
        return;
    }

    if (_cache.Count >= _capacity)
    {
        TKey oldestKey = _keysOrder.Dequeue();
        _cache.Remove(oldestKey);
    }

    _cache.Add(key, value);
    _keysOrder.Enqueue(key);
}

public TValue Get(TKey key)
{
    if (_cache.TryGetValue(key, out TValue? value))
    {
        return value;
    }

    throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші");
}
```

`Dictionary` дає швидкий доступ за ключем, а `Queue` пам'ятає порядок, якого в словнику немає. Разом обидві операції (`Add` і `Get`) працюють за O(1).

Обмеження `where TKey : notnull` додав, бо ключ словника не може бути `null`.

У `Main` показав роботу на `Cache<string, string>` (країна → столиця) з ємністю 3: після додавання 4-го і 5-го елемента видаляються найстаріші `"ua"` і `"pl"`, а `Get("ua")` викидає `KeyNotFoundException`. Також показав `Cache<int, double>`, щоб було видно, що клас узагальнений.

---

## Юніт-тести

Файл `IndependentWork7.Tests/CacheTests.cs`, 12 тестів:

| Що перевіряється | Тести |
|---|---|
| Додавання | `Add_NewItem_ItemIsStored`, `Add_ExistingKey_UpdatesValueWithoutGrowing` |
| Отримання за ключем | `Get_ExistingKeys_ReturnsCorrectValues` |
| Перевищення ємності (FIFO) | `Add_OverCapacity_RemovesOldestItem`, `Add_OverCapacitySeveralTimes_RemovesInFifoOrder`, `Add_OverCapacity_GetDoesNotChangeOrder` |
| Неіснуючий ключ | `Get_MissingKey_ThrowsKeyNotFoundException` |
| `ContainsKey` | `ContainsKey_ReturnsTrueOnlyForAddedKeys` |
| `Count` | `Count_EmptyCache_IsZero`, `Count_IncreasesWithAdd`, `Count_NeverExceedsCapacity` |
| Конструктор | `Constructor_InvalidCapacity_Throws` |

Приклад тесту на видалення найстарішого елемента:

```csharp
[Fact]
public void Add_OverCapacity_RemovesOldestItem()
{
    Cache<string, int> cache = new Cache<string, int>(2);
    cache.Add("a", 1);
    cache.Add("b", 2);

    cache.Add("c", 3);

    Assert.False(cache.ContainsKey("a"));
    Assert.True(cache.ContainsKey("b"));
    Assert.True(cache.ContainsKey("c"));
    Assert.Throws<KeyNotFoundException>(() => cache.Get("a"));
}
```

Тест `Add_OverCapacity_GetDoesNotChangeOrder` перевіряє, що це саме FIFO, а не LRU: навіть якщо до найстарішого елемента звертались через `Get`, він все одно видаляється першим.

---

## Результати

### dotnet run

![alt text](image.png)

### dotnet test

![alt text](tests.png)
