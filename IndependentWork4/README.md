# Самостійна робота №4

> **Тема:** Розширення ієрархії та наслідки змін базового класу.

---

## Зміст

- [Хід роботи](#хід-роботи)
- [Аналіз змін базового класу](#аналіз-змін-базового-класу)
- [Переваги та недоліки наслідування](#переваги-та-недоліки-наслідування)
- [Результати](#результати)

---

## Хід роботи

### 1. Створив проєкт

```bash
dotnet new console -o IndependentWork4
```

### 2. Реалізував початкову ієрархію

- `LibraryItem` — поля `_title`, `_publicationYear`, властивості `Title`, `PublicationYear`, конструктор `LibraryItem(string title, int publicationYear)` і віртуальний метод `DisplayDetails()`.
- `Book : LibraryItem` — поле `_author`, властивість `Author`, конструктор викликає `base(title, publicationYear)`, `DisplayDetails()` перевизначений і додає автора.
- `Journal : LibraryItem` — поле `_issueNumber`, властивість `IssueNumber`, конструктор через `base`, `DisplayDetails()` додає номер випуску.

У перевизначених методах спочатку викликаю `base.DisplayDetails()`, а потім дописую своє, щоб не дублювати вивід базової інформації.

### 3. Модифікував базовий клас

Додав у `LibraryItem` поле `_uniqueId`, read-only властивість `UniqueId` і новий параметр у конструктор:

```csharp
public LibraryItem(string title, int publicationYear, string uniqueId)
{
    _uniqueId = uniqueId;
    _title = title;
    _publicationYear = publicationYear;
}
```

### 4. Додав клас AudioBook

`AudioBook` успадковується від `Book` (а не від `LibraryItem`), має поле `_narrator`, властивість `Narrator`, конструктор викликає конструктор `Book`, а `DisplayDetails()` додає оповідача.

```csharp
class AudioBook : Book
{
    private string _narrator;

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
```

### 5. Демонстрація в Main

Створив `Book`, `Journal` і `AudioBook`, викликав `DisplayDetails()` для кожного. Потім поклав усі об'єкти в `List<LibraryItem>` і викликав метод у циклі — для кожного об'єкта спрацювала його власна версія (поліморфізм). Для `AudioBook` виводиться і базова інформація, і автор (від `Book`), і оповідач, тобто ланцюжок `base.DisplayDetails()` працює через усі три рівні.

---

## Аналіз змін базового класу

Після того як я змінив конструктор `LibraryItem` (додав параметр `uniqueId`), проєкт перестав компілюватися. Компілятор видав помилки в обох похідних класах:

```
Program.cs(51,69): error CS7036: There is no argument given that corresponds to the required parameter 'uniqueId' of 'LibraryItem.LibraryItem(string, int, string)'
Program.cs(73,74): error CS7036: There is no argument given that corresponds to the required parameter 'uniqueId' of 'LibraryItem.LibraryItem(string, int, string)'
```

**Що довелося змінити:**

| Клас | Було | Стало |
|---|---|---|
| `Book` | `Book(string title, int publicationYear, string author) : base(title, publicationYear)` | `Book(string title, int publicationYear, string author, string uniqueId) : base(title, publicationYear, uniqueId)` |
| `Journal` | `Journal(string title, int publicationYear, int issueNumber) : base(title, publicationYear)` | `Journal(string title, int publicationYear, int issueNumber, string uniqueId) : base(title, publicationYear, uniqueId)` |
| `Main` | `new Book("...", 1911, "...")` | `new Book("...", 1911, "...", "B-001")` |

Також `AudioBook` вже створювався з урахуванням `uniqueId`, бо він викликає конструктор `Book`, а той тепер вимагає цей параметр.

**Чому це сталося:**

- Конструктори не успадковуються. Кожен похідний клас сам викликає конструктор базового через `: base(...)`.
- Старого конструктора `LibraryItem(string, int)` більше немає, тому виклик `base(title, publicationYear)` не відповідає жодному конструктору базового класу.
- Похідний клас не може створитися без того, щоб спочатку не створилась його базова частина, тому він зобов'язаний передати всі обов'язкові параметри, включно з новим `uniqueId`.
- Зміна поширюється по всьому ланцюжку: `LibraryItem` → `Book` → `AudioBook`, і далі на всі місця в коді, де створюються об'єкти.

Методи `DisplayDetails()` у похідних класах змінювати не довелося — вони викликають `base.DisplayDetails()`, тому `UniqueId` автоматично з'явився у виводі всіх класів. Це плюс того, що спільна логіка зібрана в базовому класі.

---

## Переваги та недоліки наслідування

**Переваги:**

- Спільні поля і методи (`Title`, `PublicationYear`, `UniqueId`, `DisplayDetails()`) пишуться один раз у базовому класі.
- Нову сутність легко додати, не змінюючи існуючі класи — `AudioBook` просто успадкувався від `Book` і отримав все готове (принцип відкритості/закритості).
- Поліморфізм: усі об'єкти можна зберігати в одному `List<LibraryItem>` і обробляти однаково.

**Недоліки:**

- Сильна зв'язність: зміна в базовому класі (як з `uniqueId`) ламає всі похідні класи і код, що їх створює. Чим глибша ієрархія, тим більше правок.
- Похідні класи залежать від деталей базового класу, тому базовий клас стає важко змінювати.
- Ієрархія жорстка. Наприклад, якщо з'явиться аудіо-журнал, то його не можна успадкувати одночасно від `Journal` і `AudioBook`, бо в C# немає множинного наслідування класів.

**Коли наслідування доцільне, а коли ні:**

- Доцільне, коли є чітке відношення "є" (is-a): книга **є** бібліотечним елементом, аудіокнига **є** книгою.
- Проблеми бувають, коли наслідування використовують тільки щоб перевикористати код, або коли можливостей стає багато і вони комбінуються між собою. Тоді краще композиція: наприклад, замість класу `AudioBook` можна зробити у `Book` поле типу `AudioInfo` (оповідач, тривалість) або список форматів. Так нові можливості додаються без зміни ієрархії, а аудіо-версію можна було б додати і журналу.

---

## Результати

![alt text](image.png)
