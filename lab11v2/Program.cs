using System.Globalization;
using System.Text;

namespace Lab11v2;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}

public class OrderDetail
{
    public int Id { get; }
    public int ProductId { get; }
    public Product Product { get; }
    public int Quantity { get; }

    public OrderDetail(int id, Product product, int quantity)
    {
        Id = id;
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
    }
}

public class Order
{
    public int Id { get; }
    public DateTime OrderDate { get; }
    public List<OrderDetail> Details { get; }

    public Order(int id, DateTime orderDate, List<OrderDetail> details)
    {
        Id = id;
        OrderDate = orderDate;
        Details = details;
    }
}

public static class OrderDetailExtensions
{
    public static decimal TotalOrderAmount(this IEnumerable<OrderDetail> details)
    {
        return details.Sum(d => d.Product.Price * d.Quantity);
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var products = new List<Product>
        {
            new(1, "Laptop", 32000m),
            new(2, "Mouse", 550m),
            new(3, "Keyboard", 1200m),
            new(4, "Monitor", 8500m),
            new(5, "Headphones", 2300m),
            new(6, "Webcam", 1700m),
            new(7, "USB Cable", 150m),
            new(8, "Flash Drive", 400m),
            new(9, "Microphone", 2900m)
        };

        Product Find(int id) => products.First(p => p.Id == id);

        var orders = new List<Order>
        {
            new(1, new DateTime(2026, 11, 1), new List<OrderDetail>
            {
                new(101, Find(1), 1),
                new(102, Find(2), 2),
                new(103, Find(7), 3)
            }),
            new(2, new DateTime(2026, 11, 3), new List<OrderDetail>
            {
                new(104, Find(3), 1),
                new(105, Find(5), 1)
            }),
            new(3, new DateTime(2026, 11, 5), new List<OrderDetail>
            {
                new(106, Find(4), 2),
                new(107, Find(2), 1)
            }),
            new(4, new DateTime(2026, 11, 7), new List<OrderDetail>
            {
                new(108, Find(6), 1),
                new(109, Find(7), 2),
                new(110, Find(8), 4)
            }),
            new(5, new DateTime(2026, 11, 9), new List<OrderDetail>
            {
                new(111, Find(1), 1),
                new(112, Find(5), 2)
            }),
            new(6, new DateTime(2026, 11, 12), new List<OrderDetail>
            {
                new(113, Find(3), 2),
                new(114, Find(8), 1)
            }),
            new(7, new DateTime(2026, 11, 15), new List<OrderDetail>
            {
                new(115, Find(4), 1)
            })
        };

        Console.WriteLine("--- Запит 1: усі замовлені продукти без дублікатів (SelectMany) ---");
        int nestedCollections = orders.Select(o => o.Details).Count();
        int flatDetails = orders.SelectMany(o => o.Details).Count();
        Console.WriteLine($"Select повертає колекцій: {nestedCollections}");
        Console.WriteLine($"SelectMany повертає позицій замовлень: {flatDetails}");

        var orderedProducts = orders
            .SelectMany(o => o.Details)
            .Select(d => d.Product)
            .DistinctBy(p => p.Id)
            .ToList();

        Console.WriteLine($"Унікальних замовлених продуктів: {orderedProducts.Count}");
        foreach (var product in orderedProducts)
        {
            Console.WriteLine($"  {product.Id}. {product.Name}, {product.Price:F2} грн");
        }

        Console.WriteLine();
        Console.WriteLine("--- Запит 2: пагінація, 2 замовлення на сторінку ---");
        const int pageSize = 2;
        int totalPages = (int)Math.Ceiling(orders.Count / (double)pageSize);

        for (int page = 1; page <= totalPages; page++)
        {
            var pageOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine($"Сторінка {page} з {totalPages}");
            foreach (var order in pageOrders)
            {
                Console.WriteLine($"  Замовлення №{order.Id} від {order.OrderDate:dd.MM.yyyy}");
                foreach (var detail in order.Details)
                {
                    Console.WriteLine($"    {detail.Product.Name} x{detail.Quantity}");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine("--- Запит 3: метод розширення TotalOrderAmount() ---");
        foreach (var order in orders)
        {
            Console.WriteLine($"  Замовлення №{order.Id}: {order.Details.TotalOrderAmount():F2} грн");
        }

        decimal grandTotal = orders.SelectMany(o => o.Details).TotalOrderAmount();
        Console.WriteLine($"Загальна сума всіх замовлень: {grandTotal:F2} грн");
    }
}