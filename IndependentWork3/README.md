# Звіт з аналізу інкапсуляції в Open-Source проєкті

> **Самостійна робота №3**
> **Тема:** Аналіз інкапсуляції в open-source проєктах. Практики валідації полів.

---

## 1. Обраний проєкт

- **Назва:** Newtonsoft.Json (Json.NET) — бібліотека для серіалізації та десеріалізації JSON у .NET. Одна з найпопулярніших бібліотек на NuGet.
- **Посилання на GitHub:** https://github.com/JamesNK/Newtonsoft.Json
- **Аналізовані файли:**
  - [`Src/Newtonsoft.Json/JsonReader.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonReader.cs)
  - [`Src/Newtonsoft.Json/JsonSerializerSettings.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonSerializerSettings.cs)
  - [`Src/Newtonsoft.Json/Linq/JArray.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JArray.cs)
  - [`Src/Newtonsoft.Json/Linq/JToken.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JToken.cs) (оператори)

---

## 2. Аналіз інкапсуляції

### Клас: `JsonReader`

- **Опис класу:** абстрактний базовий клас для читання JSON по токенах (`{`, `"name"`, `123` і т.д.). Від нього успадковуються `JsonTextReader`, `JTokenReader` та інші.

- **Поля:** усі поля стану приватні або `internal`, публічних полів немає. Зовні не можна напряму змінити поточний токен чи стан читача.

```csharp
private JsonToken _tokenType;
private object? _value;
internal char _quoteChar;
internal State _currentState;
private JsonPosition _currentPosition;
private CultureInfo? _culture;
private DateTimeZoneHandling _dateTimeZoneHandling;
private int? _maxDepth;
private bool _hasExceededMaxDepth;
```

`internal` використовується для полів, які потрібні класам-нащадкам у цій же збірці (наприклад `JsonTextReader`), але користувачу бібліотеки вони не видні.

- **Властивості:**
  - автоматичні властивості для простих налаштувань без перевірок:

```csharp
public bool CloseInput { get; set; }
public bool SupportMultipleContent { get; set; }
```

  - властивість тільки для читання (expression-bodied, без `set`), стан видно, але змінити не можна:

```csharp
protected State CurrentState => _currentState;
```

  - властивість з різним рівнем доступу для `get` і `set` — читати може будь-хто, а встановлювати тільки сам клас і нащадки:

```csharp
public virtual char QuoteChar
{
    get => _quoteChar;
    protected internal set => _quoteChar = value;
}
```

  - властивості з валідацією в `set` (`MaxDepth`, `DateTimeZoneHandling`, `DateParseHandling`) — розібрані в розділі 3.

- **Індексатори/Оператори:** немає.

[Посилання на властивості в коді (рядки 133–245)](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonReader.cs#L133-L245)

### Клас: `JsonSerializerSettings`

- **Опис класу:** клас налаштувань серіалізатора (форматування, обробка null, циклічних посилань, дат, максимальна глибина і т.д.).

- **Поля:** цікавий підхід — майже всі поля мають nullable-тип (`Formatting?`, `int?` і т.д.). `null` означає "значення не задавали, використовуй за замовчуванням". Значення за замовчуванням винесені в константи.

```csharp
internal const int DefaultMaxDepth = 64;
internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;

internal Formatting? _formatting;
internal int? _maxDepth;
internal bool _maxDepthSet;
internal ReferenceLoopHandling? _referenceLoopHandling;
```

- **Властивості:** властивості read-write, але не автоматичні — у `get` повертається або задане значення, або значення за замовчуванням:

```csharp
public ReferenceLoopHandling ReferenceLoopHandling
{
    get => _referenceLoopHandling ?? DefaultReferenceLoopHandling;
    set => _referenceLoopHandling = value;
}
```

Для `MaxDepth` і `DateFormatString` додатково є прапорець `_maxDepthSet` / `_dateFormatStringSet`, бо для `MaxDepth` значення `null` теж допустиме (означає "без обмеження"), тому просто перевірити на `null` недостатньо:

```csharp
public string DateFormatString
{
    get => _dateFormatString ?? DefaultDateFormatString;
    set
    {
        _dateFormatString = value;
        _dateFormatStringSet = true;
    }
}
```

Тут інкапсуляція дозволяє користувачу працювати з простою властивістю, а вся логіка "задано / не задано" схована всередині.

- **Індексатори/Оператори:** немає.

[Посилання на клас](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonSerializerSettings.cs#L46)

### Клас: `JArray`

- **Опис класу:** представляє JSON-масив у LINQ to JSON, реалізує `IList<JToken>`.

- **Поля:** елементи масиву зберігаються в приватному `readonly` списку. Саме посилання на список змінити не можна, а доступ до елементів йде тільки через методи та індексатор класу.

```csharp
private readonly List<JToken> _values = new List<JToken>();

protected override IList<JToken> ChildrenTokens => _values;
```

- **Властивості:** `IsReadOnly` — read-only властивість, обов'язкова для інтерфейсу `ICollection<T>`:

```csharp
public bool IsReadOnly => false;
```

- **Індексатори:** є два індексатори. Звичайний по `int` і перевизначений по `object` (з базового `JToken`), який перевіряє тип ключа:

```csharp
public JToken this[int index]
{
    get => GetItem(index);
    set => SetItem(index, value);
}

public override JToken? this[object key]
{
    get
    {
        ValidationUtils.ArgumentNotNull(key, nameof(key));

        switch (key)
        {
            case int intKey:
                return GetItem(intKey);
            default:
                throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
        }
    }
    ...
}
```

Індексатор дозволяє писати `array[0]`, як зі звичайним масивом, але всередині виконується `GetItem/SetItem`, які встановлюють батьківський елемент токена і т.д. Користувач не має прямого доступу до `_values`.

- **Оператори:** перевантажені оператори є в базовому класі `JToken`. Неявні — щоб можна було писати `JToken t = 5;`, і явні — для `int x = (int)token;` з перевіркою типу:

```csharp
public static implicit operator JToken(int value)
{
    return new JValue(value);
}

public static explicit operator int(JToken value)
{
    JValue? v = EnsureValue(value);
    if (v == null || !ValidateToken(v, NumberTypes, false))
    {
        throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
    }
    ...
}
```

[JArray — індексатори](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JArray.cs#L245-L292), [JToken — оператори](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Linq/JToken.cs#L819)

---

## 3. Практики валідації у властивостях

- **Приклад 1: перевірка діапазону числа (`MaxDepth`)**

Максимальна глибина вкладеності JSON не може бути 0 або від'ємною. Перевірка через `if` з викиданням `ArgumentException`. Однаковий код є і в `JsonReader`, і в `JsonSerializerSettings`.

```csharp
public int? MaxDepth
{
    get => _maxDepth;
    set
    {
        if (value <= 0)
        {
            throw new ArgumentException("Value must be positive.", nameof(value));
        }

        _maxDepth = value;
    }
}
```

Цікаво, що для `int?` порівняння `null <= 0` дає `false`, тому `null` проходить перевірку — і це правильно, бо `null` означає "без обмеження".

[Код у JsonReader.cs](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonReader.cs#L233-L245)

- **Приклад 2: перевірка значення enum (`DateTimeZoneHandling`, `DateParseHandling`)**

У C# в enum можна записати будь-яке число через приведення, наприклад `(DateTimeZoneHandling)100`. Тому в `set` перевіряється, що значення входить у допустимий діапазон, інакше `ArgumentOutOfRangeException`.

```csharp
public DateTimeZoneHandling DateTimeZoneHandling
{
    get => _dateTimeZoneHandling;
    set
    {
        if (value < DateTimeZoneHandling.Local || value > DateTimeZoneHandling.RoundtripKind)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        _dateTimeZoneHandling = value;
    }
}
```

[Код у JsonReader.cs](https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonReader.cs#L165-L198)

- **Приклад 3: перевірка аргументу на null і на тип (`JArray`)**

В індексаторі `this[object key]` спочатку викликається допоміжний метод `ValidationUtils.ArgumentNotNull`, а потім через `switch` перевіряється, що ключ має тип `int`. Інакше — `ArgumentException` з поясненням.

- **Висновки щодо валідації:**
  - У проєкті використовуються прості `if`-перевірки + винятки `ArgumentException` / `ArgumentOutOfRangeException` з `nameof(value)`. Атрибутів `DataAnnotations` немає — це бібліотека, а не модель з форми, тому перевірка в `set` логічніша.
  - Плюс: неправильне значення не потрапляє в поле, помилка виникає одразу в місці присвоєння, а не пізніше під час читання JSON, де її важче знайти.
  - Плюс: повторювані перевірки (на null) винесені в окремий клас `ValidationUtils`.
  - Мінус: перевірка `MaxDepth` продубльована в двох класах.
  - Валідуються не всі властивості — `CloseInput`, `SupportMultipleContent`, `ReferenceLoopHandling` в `JsonSerializerSettings` залишені без перевірок, бо для `bool` будь-яке значення допустиме, а для деяких enum перевірку, мабуть, просто не додали.

---

## 4. Загальні висновки

- У всіх розглянутих класах немає публічних полів, тільки `private` / `internal`. Доступ іде через властивості, індексатори та методи, тобто інкапсуляція дотримана.
- Використовуються різні види властивостей залежно від задачі: автоматичні — для простих прапорців, read-only — для стану, з різним доступом `get`/`protected set` — для даних, які можуть змінювати тільки нащадки, і з валідацією — там, де значення можна передати некоректне.
- `JsonSerializerSettings` показує, що властивість може приховувати складнішу логіку (значення за замовчуванням, прапорець "задано"), а користувач бачить просту властивість.
- Індексатори та оператори в `JArray`/`JToken` роблять роботу з JSON схожою на роботу зі звичайними масивами та числами, але при цьому всі перевірки виконуються всередині класу.
- Загалом підходи доцільні: код зрозумілий, некоректні дані відсікаються одразу, а внутрішню реалізацію можна змінювати, не ламаючи код користувачів бібліотеки.
