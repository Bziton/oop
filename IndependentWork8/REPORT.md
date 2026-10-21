# Звіт: Dictionary<TKey, TValue> та HashSet<T> у бібліотеках .NET

> **Самостійна робота №8**

---

## 1. Результати аналізу open-source проєктів

Для аналізу взяв дві популярні бібліотеки:

- **Newtonsoft.Json** — https://github.com/JamesNK/Newtonsoft.Json
- **Dapper** — https://github.com/DapperLib/Dapper

### Приклад 1: Dictionary — таблиця типів у `ConvertUtils` (Newtonsoft.Json)

Файл: [`Src/Newtonsoft.Json/Utilities/ConvertUtils.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ConvertUtils.cs#L125)

```csharp
private static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap =
    new Dictionary<Type, PrimitiveTypeCode>
    {
        { typeof(char), PrimitiveTypeCode.Char },
        { typeof(char?), PrimitiveTypeCode.CharNullable },
        { typeof(bool), PrimitiveTypeCode.Boolean },
        { typeof(bool?), PrimitiveTypeCode.BooleanNullable },
        ...
    };

public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
{
    if (TypeCodeMap.TryGetValue(t, out PrimitiveTypeCode typeCode))
    {
        isEnum = false;
        return typeCode;
    }
    ...
}
```

- **Контекст:** при серіалізації бібліотеці постійно треба знати, чи є тип примітивним (int, bool, DateTime, Guid...) і яким саме, щоб обрати правильний спосіб запису.
- **Чому Dictionary:** це таблиця відповідності "тип → код". Через `List` довелось би перебирати ~40 пар для кожного значення, яке серіалізується. `switch` по `Type` теж не зробиш.
- **Складність:** `TryGetValue` — O(1) в середньому.
- **Особливості:** словник `static readonly` — заповнюється один раз і тільки читається, тому безпечний для кількох потоків. `TryGetValue` замість індексатора, бо відсутність типу в таблиці — нормальна ситуація (тоді перевіряється, чи це enum).

### Приклад 2: HashSet — злиття масивів без дублікатів у `JContainer` (Newtonsoft.Json)

Файл: [`Src/Newtonsoft.Json/Linq/JContainer.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JContainer.cs#L1224)

```csharp
case MergeArrayHandling.Union:
#if HAVE_HASH_SET
    HashSet<JToken> items = new HashSet<JToken>(target, EqualityComparer);

    foreach (object item in content)
    {
        JToken contentItem = CreateFromContent(item);

        if (items.Add(contentItem))
        {
            target.Add(contentItem);
        }
    }
#else
    Dictionary<JToken, bool> items = new Dictionary<JToken, bool>(EqualityComparer);
    ...
#endif
```

- **Контекст:** метод `JArray.Merge()` з режимом `Union` — до масиву додаються тільки ті елементи, яких там ще немає.
- **Чому HashSet:** потрібно швидко перевіряти "чи є вже такий елемент". `HashSet.Add()` одразу і перевіряє, і додає, повертаючи `false` для дубліката. З `List.Contains()` злиття двох масивів по n елементів було б O(n²).
- **Складність:** `Add` — O(1), все злиття — O(n + m).
- **Особливості:** використовується **custom comparer** `JTokenEqualityComparer`, який порівнює токени за вмістом (`JToken.DeepEquals`), а не за посиланням. Без нього два однакові об'єкти `{"a":1}` вважались би різними. Цікаво, що для старих версій .NET без `HashSet` автор використовує `Dictionary<JToken, bool>` як заміну — значення `bool` там не потрібне, використовуються тільки ключі.

### Приклад 3: Dictionary + HashSet разом — позиційні параметри в Dapper

Файл: [`Dapper/SqlMapper.cs`, метод `PassByPosition`](https://github.com/DapperLib/Dapper/blob/main/Dapper/SqlMapper.cs#L1907)

```csharp
private static void PassByPosition(IDbCommand cmd)
{
    if (cmd.Parameters.Count == 0) return;

    Dictionary<string, IDbDataParameter> parameters = new(StringComparer.Ordinal);

    foreach (IDbDataParameter param in cmd.Parameters)
    {
        if (!string.IsNullOrEmpty(param.ParameterName)) parameters[param.ParameterName] = param;
    }
    var consumed = new HashSet<string>(StringComparer.Ordinal);
    ...
    cmd.CommandText = CompiledRegex.PseudoPositional.Replace(cmd.CommandText, match =>
    {
        string key = match.Groups[1].Value;
        if (!consumed.Add(key))
        {
            throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
        }
        else if (parameters.TryGetValue(key, out IDbDataParameter? param))
        {
            ...
            cmd.Parameters.Add(param);
            parameters.Remove(key);
            return "?";
        }
        ...
    });
}
```

- **Контекст:** деякі провайдери БД (наприклад, ODBC) підтримують тільки позиційні параметри `?`. Dapper бере SQL з іменами типу `?id?`, знаходить для кожного імені параметр і замінює на `?` у правильному порядку.
- **Чому обидві колекції:**
  - `Dictionary<string, IDbDataParameter>` — швидко знайти об'єкт параметра за ім'ям, яке знайдено в тексті SQL;
  - `HashSet<string> consumed` — запам'ятати, які імена вже використані, щоб кинути помилку, якщо параметр зустрічається двічі.
- **Складність:** кожна перевірка і пошук O(1), увесь метод — O(k) від кількості збігів у SQL.
- **Особливості:** обидві колекції з `StringComparer.Ordinal` — порівняння рядків побайтово, без урахування культури (швидше і передбачуваніше для імен). Знову використовується трюк `if (!consumed.Add(key))` — перевірка і додавання за одну операцію.

**Ще один приклад custom comparer:** у Newtonsoft.Json властивості `JObject` зберігаються у `Dictionary<string, JToken>(StringComparer.Ordinal)` ([JPropertyKeyedCollection.cs](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JPropertyKeyedCollection.cs#L110)), тобто `"Name"` і `"name"` — різні ключі, як і в самому JSON.

---

## 2. Порівняльна таблиця

| Операція | List\<T\> | Dictionary<TKey,TValue> | HashSet\<T\> |
|---|---|---|---|
| Додавання | O(1)* в кінець, O(n) у середину | O(1)* | O(1)* |
| Пошук за значенням | O(n) | N/A (O(n) через `ContainsValue`) | O(1) |
| Пошук за ключем | N/A (O(1) за індексом) | O(1) | N/A |
| Видалення | O(n) | O(1) | O(1) |
| Перевірка наявності | O(n) (`Contains`) | O(1) (`ContainsKey`) | O(1) (`Contains`) |
| Порядок елементів | зберігається | не гарантується | не гарантується |
| Дублікати | дозволені | ключі унікальні | заборонені |
| **Коли використовувати** | важливий порядок, доступ за індексом, мало пошуків | треба знайти значення за ключем (кеш, індекс, таблиця відповідності) | треба унікальність і швидка перевірка "чи є", операції над множинами |

\* амортизовано: інколи при розширенні внутрішнього масиву/таблиці буде O(n).

---

## 3. Висновки про вибір колекції

**Коли Dictionary краще за List:**

- коли дані шукаються за ключем (ім'я файлу, id, тип) — O(1) замість O(n);
- коли часто перевіряється наявність або видаляються елементи;
- у моєму `FileMetadataCache` метадані шукаються за ім'ям файлу, тому `Dictionary` підходить ідеально.

**Коли HashSet є оптимальним вибором:**

- коли потрібні тільки унікальні значення без зайвих даних (теги, вже оброблені ключі);
- коли потрібні операції над множинами: `UnionWith`, `IntersectWith`, `ExceptWith`;
- як "журнал відвіданих" (як `consumed` у Dapper) — `Add` повертає `false` для повтору.

**Підводні камені:**

- **Немає порядку.** `Dictionary` і `HashSet` не гарантують порядок елементів. У своєму коді для виводу я сортую результат.
- **Регістр рядків.** За замовчуванням `"CSharp"` і `"csharp"` — різні ключі. Потрібно явно передавати `StringComparer.OrdinalIgnoreCase` (я так зробив для тегів, імен файлів і слів в індексі).
- **Власні класи як ключі.** Без перевизначених `Equals()`/`GetHashCode()` або comparer порівнюються посилання — два однакові об'єкти будуть різними ключами (тому Newtonsoft передає `JTokenEqualityComparer`).
- **Змінювані ключі.** Якщо змінити поле об'єкта, від якого залежить `GetHashCode()`, після додавання в колекцію — його вже не знайти.
- **Індексатор кидає виняток.** `dict[key]` для неіснуючого ключа дає `KeyNotFoundException`, тому для пошуку краще `TryGetValue` (так роблять і Newtonsoft, і Dapper).
- **Зміна під час перебору.** Не можна додавати/видаляти елементи в `foreach` по тій самій колекції — буде `InvalidOperationException`.
- **Повернення внутрішньої колекції.** Якщо метод повертає свій внутрішній `HashSet`, а викликач викличе на ньому `IntersectWith`, зміниться стан класу. Тому в `TagManager` і `SearchIndex` я повертаю копію.

---

## 4. Приклади з власного коду

Код усіх трьох сценаріїв — у `Program.cs`, вивід програми — у [README.md](README.md).

### Сценарій 1: FileMetadataCache (Dictionary)

```csharp
private Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

public FileMetadata? Get(string fileName)
{
    if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
    {
        return metadata;
    }
    return null;
}
```

Ключ — ім'я файлу без урахування регістру (як у Windows), тому `Get("program.cs")` знаходить `Program.cs`. `Add`, `Get`, `Remove`, `Contains` — O(1).

### Сценарій 2: TagManager (HashSet)

```csharp
public HashSet<string> GetAllUniqueTags()
{
    HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
```

Для кожної статті — свій `HashSet` тегів, тому тег `"CSharp"` не додається повторно, якщо вже є `"csharp"`. Спільні теги — `IntersectWith`, всі теги — `UnionWith`, різниця — `ExceptWith`.

### Сценарій 3: SearchIndex (Dictionary + HashSet)

```csharp
private Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

public HashSet<int> SearchMultiple(params string[] words)
{
    if (words.Length == 0)
    {
        return new HashSet<int>();
    }

    HashSet<int> result = Search(words[0]);
    for (int i = 1; i < words.Length; i++)
    {
        result.IntersectWith(Search(words[i]));
    }
    return result;
}
```

Це інвертований індекс, як у пошукових системах: слово → множина номерів документів. `Dictionary` дає швидкий пошук слова, `HashSet` гарантує, що документ не запишеться двічі, якщо слово в ньому повторюється, а `IntersectWith` знаходить документи, де є **всі** слова.
