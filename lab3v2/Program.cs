using System;
using System.Runtime.CompilerServices;
using System.Text;
 
public class DatabaseConnection : IDisposable
{
    private string _connectionString;
    private bool _isConnected;
    private bool _disposed = false;
 
    public bool IsConnected
    {
        get { return _isConnected; }
    }

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
        _isConnected = true;
        Console.WriteLine("Connected to: " + _connectionString);
    }
 
    public void ExecuteQuery(string query)
    {
        if (_isConnected)
        {
            Console.WriteLine("Executing: " + query);
        }
        else
        {
            Console.WriteLine("Cannot execute query. Not connected to the database.");
        }
    }
 
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Звільнення керованих ресурсів
                Console.WriteLine("Disposing managed resources");
            }
 
            // Звільнення некерованих ресурсів
            if (_isConnected)
            {
                Console.WriteLine("Closing connection");
                _isConnected = false;
            }
 
            _disposed = true;
        }
    }
 
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
 
    ~DatabaseConnection()
    {
        Dispose(false);
    }
}
 
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
 
        Console.WriteLine("--- Сценарій A: using ---");
        using (var db = new DatabaseConnection("Server=localhost;Database=test"))
        {
            db.ExecuteQuery("SELECT * FROM Users");
        }

        Console.WriteLine();
        Console.WriteLine("--- Сценарій B: ручний Dispose() ---");
        var db2 = new DatabaseConnection("Server=localhost;Database=test");
        try
        {
            db2.ExecuteQuery("INSERT INTO Users VALUES (1, 'Ivan')");
        }
        finally
        {
            db2.Dispose();
        }
        db2.ExecuteQuery("SELECT 1");
 
        Console.WriteLine();
        Console.WriteLine("--- Сценарій C: деструктор через GC ---");
        CreateWithoutDispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
 
        Console.WriteLine();
        Console.WriteLine("Кінець програми");
    }
 
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void CreateWithoutDispose()
    {
        var db3 = new DatabaseConnection("Server=localhost;Database=test");
        db3.ExecuteQuery("DELETE FROM Users WHERE Id = 1");
    }
}