namespace IndependentWork11;

public static class DataGenerator
{
    private static readonly string[] Categories =
        { "Electronics", "Books", "Clothing", "Home", "Sports" };

    private static readonly string[][] ProductNames =
    {
        new[] { "Laptop", "Smartphone", "Tablet", "Headphones", "Monitor", "Keyboard",
                "Mouse", "Webcam", "Smartwatch", "Speaker", "Router", "Charger" },
        new[] { "Novel", "Cookbook", "Atlas", "Dictionary", "Biography", "Comics",
                "Textbook", "Poetry", "Thriller", "Fantasy", "Guidebook", "Encyclopedia" },
        new[] { "T-Shirt", "Jeans", "Jacket", "Sweater", "Sneakers", "Dress",
                "Scarf", "Hat", "Socks", "Coat", "Shorts", "Gloves" },
        new[] { "Lamp", "Pillow", "Blanket", "Kettle", "Vacuum", "Curtains",
                "Mirror", "Clock", "Toaster", "Frying Pan", "Carpet", "Vase" },
        new[] { "Football", "Tennis Racket", "Dumbbells", "Yoga Mat", "Bicycle", "Helmet",
                "Jump Rope", "Skateboard", "Backpack", "Water Bottle", "Boxing Gloves", "Swim Goggles" }
    };

    private static readonly string[] FirstNames =
        { "Andrii", "Olena", "Dmytro", "Iryna", "Taras", "Maria", "Oleksandr", "Natalia", "Bohdan", "Sofiia" };

    private static readonly string[] LastNames =
        { "Kovalenko", "Shevchenko", "Bondar", "Melnyk", "Tkachenko", "Kravchuk", "Savchenko", "Polishchuk" };

    private static readonly string[] Cities =
        { "Rivne", "Kyiv", "Lviv", "Odesa", "Kharkiv", "Dnipro" };

    public static List<Product> GenerateProducts(Random rnd)
    {
        var products = new List<Product>();
        int id = 1;

        for (int c = 0; c < Categories.Length; c++)
        {
            foreach (string name in ProductNames[c])
            {
                decimal price = rnd.Next(50, 5001);
                products.Add(new Product(id, name, Categories[c], price));
                id++;
            }
        }

        return products;
    }

    public static List<Customer> GenerateCustomers(Random rnd)
    {
        var customers = new List<Customer>();

        for (int i = 0; i < FirstNames.Length * LastNames.Length; i++)
        {
            string name = $"{FirstNames[i % FirstNames.Length]} {LastNames[i / FirstNames.Length]}";
            string city = Cities[rnd.Next(Cities.Length)];
            customers.Add(new Customer(i + 1, name, city));
        }

        return customers;
    }

    public static List<Sale> GenerateSales(Random rnd, int productCount, int customerCount, int count)
    {
        var sales = new List<Sale>();
        var startDate = new DateTime(2024, 1, 1);

        for (int i = 1; i <= count; i++)
        {
            int productId = rnd.Next(1, productCount + 1);
            int quantity = rnd.Next(1, 11);
            int dayOffset = rnd.Next(0, 730);
            int customerId = rnd.Next(1, customerCount + 1);
            sales.Add(new Sale(i, productId, quantity, startDate.AddDays(dayOffset), customerId));
        }

        return sales;
    }
}