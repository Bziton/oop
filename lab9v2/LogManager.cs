using System;
using System.Collections.Generic;

public class LogManager
{
    private readonly HashSet<string> _uniqueIps = new HashSet<string>(StringComparer.Ordinal);
    private readonly Queue<LogEntry> _pendingLogs = new Queue<LogEntry>();
    private readonly Stack<LogEntry> _viewHistory = new Stack<LogEntry>();
    private readonly List<LogEntry> _processedLogs = new List<LogEntry>();

    public int PendingCount => _pendingLogs.Count;
    public int ProcessedCount => _processedLogs.Count;
    public int HistoryCount => _viewHistory.Count;
    public int UniqueIpCount => _uniqueIps.Count;

    public void Enqueue(LogEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        _pendingLogs.Enqueue(entry);
    }

    public LogEntry? PeekNext()
    {
        return _pendingLogs.TryPeek(out LogEntry? entry) ? entry : null;
    }

    public LogEntry? ProcessNext(out bool isNewIp)
    {
        isNewIp = false;

        if (!_pendingLogs.TryDequeue(out LogEntry? entry))
        {
            return null;
        }

        isNewIp = _uniqueIps.Add(entry.IpAddress);
        _processedLogs.Add(entry);
        return entry;
    }

    public bool HasSeenIp(string ipAddress)
    {
        return _uniqueIps.Contains(ipAddress);
    }

    public bool RemoveIp(string ipAddress)
    {
        return _uniqueIps.Remove(ipAddress);
    }

    public HashSet<string> GetUniqueIps()
    {
        return new HashSet<string>(_uniqueIps, StringComparer.Ordinal);
    }

    public HashSet<string> FindBlacklistedIps(IEnumerable<string> blacklist)
    {
        HashSet<string> result = GetUniqueIps();
        result.IntersectWith(blacklist);
        return result;
    }

    public HashSet<string> ExcludeIps(IEnumerable<string> excluded)
    {
        HashSet<string> result = GetUniqueIps();
        result.ExceptWith(excluded);
        return result;
    }

    public LogEntry? View(int logId)
    {
        LogEntry? entry = _processedLogs.Find(log => log.Id == logId);

        if (entry != null)
        {
            _viewHistory.Push(entry);
        }

        return entry;
    }

    public LogEntry? PeekLastViewed()
    {
        return _viewHistory.TryPeek(out LogEntry? entry) ? entry : null;
    }

    public LogEntry? GoBack()
    {
        return _viewHistory.TryPop(out LogEntry? entry) ? entry : null;
    }
}