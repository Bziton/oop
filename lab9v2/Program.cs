using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var manager = new LogManager();

        DemoQueue(manager);
        Console.WriteLine();
        DemoHashSet(manager);
        Console.WriteLine();
        DemoStack(manager);
        Console.WriteLine();
        DemoPerformance();
    }

    static string Join(IEnumerable<string> items)
    {
        var list = new List<string>(items);
        list.Sort(StringComparer.Ordinal);
        return list.Count == 0 ? "(порожньо)" : string.Join(", ", list);
    }

    static void DemoQueue(LogManager manager)
    {
        Console.WriteLine("=== Queue<LogEntry>: черга логів для обробки (FIFO) ===");

        DateTime start = new DateTime(2026, 11, 5, 9, 0, 0);

        manager.Enqueue(new LogEntry(1, start.AddSeconds(0), "192.168.0.10", "INFO", "Користувач увійшов у систему"));
        manager.Enqueue(new LogEntry(2, start.AddSeconds(5), "10.0.0.5", "WARN", "Повільний запит до бази даних"));
        manager.Enqueue(new LogEntry(3, start.AddSeconds(9), "192.168.0.10", "INFO", "Перегляд сторінки /orders"));
        manager.Enqueue(new LogEntry(4, start.AddSeconds(14), "172.16.4.8", "ERROR", "Помилка авторизації"));
        manager.Enqueue(new LogEntry(5, start.AddSeconds(20), "10.0.0.5", "ERROR", "Тайм-аут з'єднання"));
        manager.Enqueue(new LogEntry(6, start.AddSeconds(27), "203.0.113.77", "WARN", "Підозріла кількість запитів"));

        Console.WriteLine($"У черзі логів: {manager.PendingCount}");
        Console.WriteLine($"Peek (перший у черзі): {manager.PeekNext()}");
        Console.WriteLine("Обробка в порядку надходження:");

        while (manager.PendingCount > 0)
        {
            LogEntry? entry = manager.ProcessNext(out bool isNewIp);
            Console.WriteLine($"  Dequeue -> {entry}  {(isNewIp ? "[нова IP]" : "[IP вже була]")}");
        }

        Console.WriteLine($"Залишилось у черзі: {manager.PendingCount}, оброблено: {manager.ProcessedCount}");
        Console.WriteLine($"Peek порожньої черги: {(manager.PeekNext()?.ToString() ?? "null")}");
    }

    static void DemoHashSet(LogManager manager)
    {
        Console.WriteLine("=== HashSet<string>: унікальні IP-адреси ===");

        Console.WriteLine($"Оброблено логів: {manager.ProcessedCount}, унікальних IP: {manager.UniqueIpCount}");
        Console.WriteLine($"Унікальні IP: {Join(manager.GetUniqueIps())}");
        Console.WriteLine($"Contains(\"10.0.0.5\"): {manager.HasSeenIp("10.0.0.5")}");
        Console.WriteLine($"Contains(\"8.8.8.8\"): {manager.HasSeenIp("8.8.8.8")}");

        var blacklist = new List<string> { "203.0.113.77", "198.51.100.1" };
        Console.WriteLine($"Чорний список: {Join(blacklist)}");
        Console.WriteLine($"IntersectWith (IP з чорного списку серед логів): {Join(manager.FindBlacklistedIps(blacklist))}");

        var internalIps = new List<string> { "192.168.0.10", "10.0.0.5" };
        Console.WriteLine($"ExceptWith (без внутрішніх IP): {Join(manager.ExcludeIps(internalIps))}");

        Console.WriteLine($"Remove(\"172.16.4.8\"): {manager.RemoveIp("172.16.4.8")}");
        Console.WriteLine($"Remove(\"172.16.4.8\") повторно: {manager.RemoveIp("172.16.4.8")}");
        Console.WriteLine($"Унікальних IP після видалення: {manager.UniqueIpCount}");
    }

    static void DemoStack(LogManager manager)
    {
        Console.WriteLine("=== Stack<LogEntry>: історія переглянутих логів (LIFO) ===");

        foreach (int id in new[] { 1, 4, 6, 2 })
        {
            Console.WriteLine($"  Push -> {manager.View(id)}");
        }

        Console.WriteLine($"У історії: {manager.HistoryCount}");
        Console.WriteLine($"Peek (вершина стеку): {manager.PeekLastViewed()}");
        Console.WriteLine("Повернення назад (Pop) - від останнього до першого:");

        while (manager.HistoryCount > 0)
        {
            Console.WriteLine($"  Pop <- {manager.GoBack()}");
        }

        Console.WriteLine($"Pop порожнього стеку: {(manager.GoBack()?.ToString() ?? "null")}");
    }

    static void DemoPerformance()
    {
        Console.WriteLine("=== Порівняння продуктивності: List<string> та HashSet<string> ===");

        const int ipCount = 20000;
        const int lookups = 20000;

        var list = new List<string>(ipCount);
        var set = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < ipCount; i++)
        {
            string ip = $"10.{i / 65536}.{(i / 256) % 256}.{i % 256}";
            list.Add(ip);
            set.Add(ip);
        }

        var stopwatch = new Stopwatch();
        int foundInList = 0;
        int foundInSet = 0;

        stopwatch.Start();
        for (int i = 0; i < lookups; i++)
        {
            string ip = $"10.{(ipCount - 1 - i) / 65536}.{((ipCount - 1 - i) / 256) % 256}.{(ipCount - 1 - i) % 256}";
            if (list.Contains(ip))
            {
                foundInList++;
            }
        }
        stopwatch.Stop();
        long listMs = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        for (int i = 0; i < lookups; i++)
        {
            string ip = $"10.{(ipCount - 1 - i) / 65536}.{((ipCount - 1 - i) / 256) % 256}.{(ipCount - 1 - i) % 256}";
            if (set.Contains(ip))
            {
                foundInSet++;
            }
        }
        stopwatch.Stop();
        long setMs = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"Елементів: {ipCount}, перевірок Contains: {lookups}");
        Console.WriteLine($"List<string>.Contains: знайдено {foundInList}, час: {listMs} мс (O(n))");
        Console.WriteLine($"HashSet<string>.Contains: знайдено {foundInSet}, час: {setMs} мс (O(1))");
        Console.WriteLine("Час залежить від комп'ютера, але на великих обсягах HashSet значно швидший за List.");
    }
}