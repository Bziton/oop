using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Product
{
    private string _name;
    private decimal _price;

    public string Name
    {
        get { return _name; }
    }

    public decimal Price
    {
        get { return _price; }
    }

    public Product(string name, decimal price)
    {
        _name = name;
        _price = price;
    }

    public override string ToString()
    {
        return $"Товар: {Name}, ціна: {Price} грн";
    }
}

class Customer
{
    private string _fullName;
    private string _email;

    public string FullName
    {
        get { return _fullName; }
    }

    public string Email
    {
        get { return _email; }
    }

    public Customer(string fullName, string email)
    {
        _fullName = fullName;
        _email = email;
    }

    public override string ToString()
    {
        return $"Клієнт: {FullName} ({Email})";
    }
}

class Order
{
    private int _number;
    private Customer _customer;
    private List<Product> _products;

    public int Number
    {
        get { return _number; }
    }

    public Order(int number, Customer customer, List<Product> products)
    {
        _number = number;
        _customer = customer;
        _products = products;
    }

    public decimal GetTotal()
    {
        decimal total = 0;
        foreach (Product product in _products)
        {
            total += product.Price;
        }
        return total;
    }

    public override string ToString()
    {
        return $"Замовлення №{Number}: {_customer.FullName}, товарів: {_products.Count}, сума: {GetTotal()} грн";
    }
}

class Warehouse
{
    private string _city;

    public Warehouse(string city)
    {
        _city = city;
    }
}

class MyCustomCollection : IEnumerable<string>
{
    private List<string> _items = new List<string>();

    public int Count
    {
        get { return _items.Count; }
    }

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

class Program
{
    static void ProcessStream(Stream stream)
    {
        Console.WriteLine($"Тип потоку: {stream.GetType().Name}, довжина: {stream.Length} байт");

        stream.Position = 0;
        byte[] buffer = new byte[16];
        int read = stream.Read(buffer, 0, buffer.Length);

        Console.WriteLine($"Прочитано {read} байт: {Encoding.UTF8.GetString(buffer, 0, read)}");
    }

    static void PrintAll(IEnumerable<string> items)
    {
        foreach (string item in items)
        {
            Console.WriteLine($" - {item}");
        }
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== 1. Поліморфізм через System.Object (ToString) ===");
        Product laptop = new Product("Ноутбук", 32000);
        Product mouse = new Product("Мишка", 650);
        Customer customer = new Customer("Іван Петренко", "petrenko@gmail.com");
        Order order = new Order(1024, customer, new List<Product> { laptop, mouse });

        List<object> objects = new List<object> { laptop, customer, order, new Warehouse("Рівне"), 42, "просто рядок" };
        foreach (object obj in objects)
        {
            Console.WriteLine($"{obj.GetType().Name,-10} -> {obj}");
        }
        Console.WriteLine();

        Console.WriteLine("=== 2. Поліморфізм через IEnumerable<string> ===");
        MyCustomCollection collection = new MyCustomCollection();
        collection.Add("C#");
        collection.Add("Java");
        collection.Add("Python");

        Console.WriteLine($"MyCustomCollection ({collection.Count} елементи) через foreach:");
        foreach (string item in collection)
        {
            Console.WriteLine($" - {item}");
        }

        Console.WriteLine("Той самий метод PrintAll для MyCustomCollection, List<string> та string[]:");
        PrintAll(collection);
        PrintAll(new List<string> { "List 1", "List 2" });
        PrintAll(new string[] { "Array 1", "Array 2" });
        Console.WriteLine();

        Console.WriteLine("=== 3. Поліморфізм через абстрактний клас Stream ===");
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
    }
}
