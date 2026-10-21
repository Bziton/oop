# Самостійна робота №8

> **Тема:** Вивчення Dictionary<TKey, TValue> та HashSet<T> у бібліотеках .NET.

---

## Зміст

- [Хід роботи](#хід-роботи)
- [Звіт](#звіт)
- [Приклад виводу програми](#приклад-виводу-програми)

---

## Хід роботи

### 1. Створив проєкт

```bash
dotnet new console -o IndependentWork8
```

### 2. Реалізував три сценарії

- **`FileMetadataCache`** — кеш метаданих файлів на `Dictionary<string, FileMetadata>` з `StringComparer.OrdinalIgnoreCase`. Методи `Add`, `Get`, `Remove`, `Contains`. Клас `FileMetadata` зберігає ім'я, розмір, дату зміни і хеш (перші 8 символів MD5), допоміжний метод `FromText` створює метадані з тексту.
- **`TagManager`** — теги статей: `Dictionary<string, HashSet<string>>` (стаття → множина тегів). Методи `AddTag`, `RemoveTag`, `GetTags`, `GetAllUniqueTags` (`UnionWith`), `FindCommonTags` (`IntersectWith`), `FindUniqueTags` (`ExceptWith`). Дублікати тегів (у тому числі в іншому регістрі) не додаються.
- **`SearchIndex`** — інвертований індекс `Dictionary<string, HashSet<int>>` (слово → номери документів). `AddDocument` розбиває текст на слова, `Search` шукає одне слово, `SearchMultiple` — документи, де є всі слова (перетин множин).

```csharp
public void AddDocument(int docId, string content)
{
    char[] separators = { ' ', ',', '.', '!', '?', ':', ';', '-', '\n' };
    string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

    foreach (string word in words)
    {
        if (!_invertedIndex.TryGetValue(word, out HashSet<int>? docs))
        {
            docs = new HashSet<int>();
            _invertedIndex[word] = docs;
        }
        docs.Add(docId);
    }
}
```

---

## Звіт

Аналіз прикладів з open-source проєктів (Newtonsoft.Json, Dapper), порівняльна таблиця складності і висновки — у файлі [REPORT.md](REPORT.md).

---

## Приклад виводу програми

```
=== Сценарій 1: FileMetadataCache (Dictionary) ===
У кеші файлів: 3
Get("program.cs") (інший регістр): Program.cs, 40 байт, змінено 21.10.2026 09:15, hash 727A968C
Contains("photo.png") = True
Remove("photo.png") = True
Contains("photo.png") = False
Get("missing.txt") = null

=== Сценарій 2: TagManager (HashSet) ===
  Тег 'CSharp' вже є у статті post1, дублікат не додано
Теги post1: { csharp, dotnet, oop }
Теги post2: { collections, csharp, oop }
Теги post3 (після RemoveTag "temp"): { python }
Усі унікальні теги (UnionWith): { collections, csharp, dotnet, oop, python }
Спільні теги post1 і post2 (IntersectWith): { csharp, oop }
Є в post1, але немає в post2 (ExceptWith): { dotnet }

=== Сценарій 3: SearchIndex (Dictionary + HashSet) ===
Слів в індексі: 19
Search("зберігає") -> документи { 1, 2, 4 }
Search("dictionary") -> документи { 1, 3 }
SearchMultiple("зберігає", "значення") -> документи { 1, 2, 4 }
SearchMultiple("Dictionary", "HashSet") -> документи { 3 }
Search("java") -> документи { }
```

![alt text](image.png)
