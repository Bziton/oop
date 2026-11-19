namespace IndependentWork11;

public class Sale
{
    public int Id { get; }
    public int ProductId { get; }
    public int Quantity { get; }
    public DateTime SaleDate { get; }
    public int CustomerId { get; }

    public Sale(int id, int productId, int quantity, DateTime saleDate, int customerId)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        SaleDate = saleDate;
        CustomerId = customerId;
    }
}

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal UnitPrice { get; }

    public Product(int id, string name, string category, decimal unitPrice)
    {
        Id = id;
        Name = name;
        Category = category;
        UnitPrice = unitPrice;
    }
}

public class Customer
{
    public int Id { get; }
    public string Name { get; }
    public string City { get; }

    public Customer(int id, string name, string city)
    {
        Id = id;
        Name = name;
        City = city;
    }
}

public record ProductQuantity(string Name, string Category, int TotalQuantity);

public record CategoryTop(string Category, List<ProductQuantity> TopProducts);

public record CityRevenue(string City, decimal TotalRevenue, int SalesCount);

public record MonthlySales(int Year, int Month, int TotalQuantity, decimal TotalRevenue);

public record TopCustomer(string Name, string City, decimal TotalSpent, string FavoriteCategory);