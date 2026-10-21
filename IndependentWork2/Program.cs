using System;
using System.Globalization;
using System.Text;

class Product
{
    private int _id;
    private string _name;
    private decimal _price;
    private string _category;
    private int _stockCount;

    public int Id
    {
        get { return _id; }
    }

    public string Name
    {
        get { return _name; }
    }

    public decimal Price
    {
        get { return _price; }
    }

    public string Category
    {
        get { return _category; }
    }

    public int StockCount
    {
        get { return _stockCount; }
    }

    public Product(int id, string name, decimal price, string category, int stockCount)
    {
        _id = id;
        _name = name;
        _price = price;
        _category = category;
        _stockCount = stockCount;
    }

    public Product(int id, string name, decimal price) : this(id, name, price, "Uncategorized", 0)
    {
    }

    public Product(Product other) : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
    {
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Console.WriteLine("Створення товарів");
        Console.WriteLine();

        Product product1 = new Product(101, "Laptop", 35000m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {product1}");

        Product product2 = new Product(102, "Mouse", 800m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");

        Product product3 = new Product(product1);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");

        Console.WriteLine();
        Console.WriteLine($"product1 і product3 - різні об'єкти: {!ReferenceEquals(product1, product3)}");
    }
}
