using System;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }

    public LogEntry(int id, DateTime timestamp, string ipAddress, string level, string message)
    {
        Id = id;
        Timestamp = timestamp;
        IpAddress = ipAddress;
        Level = level;
        Message = message;
    }

    public override string ToString()
    {
        return $"#{Id} [{Timestamp:HH:mm:ss}] {Level,-5} {IpAddress,-15} {Message}";
    }
}