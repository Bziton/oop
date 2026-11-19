namespace IndependentWork11;

public static class SalesPipelines
{
    public static List<CategoryTop> TopProductsByCategory(List<Sale> sales, List<Product> products)
    {
        return sales
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
            .Join(products,
                  t => t.ProductId,
                  p => p.Id,
                  (t, p) => new ProductQuantity(p.Name, p.Category, t.TotalQuantity))
            .GroupBy(pq => pq.Category)
            .Select(g => new CategoryTop(
                g.Key,
                g.OrderByDescending(pq => pq.TotalQuantity)
                 .ThenBy(pq => pq.Name, StringComparer.Ordinal)
                 .Take(3)
                 .ToList()))
            .OrderBy(ct => ct.Category, StringComparer.Ordinal)
            .ToList();
    }

    public static List<CityRevenue> RevenueByCity(
        List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers,
                  x => x.CustomerId,
                  c => c.Id,
                  (x, c) => new { c.City, x.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Sum(x => x.Revenue), g.Count()))
            .OrderByDescending(cr => cr.TotalRevenue)
            .ThenBy(cr => cr.City, StringComparer.Ordinal)
            .ToList();
    }

    public static List<MonthlySales> SalesByMonth(List<Sale> sales, List<Product> products)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .Select(g => new MonthlySales(
                g.Key.Year,
                g.Key.Month,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    public static List<TopCustomer> TopCustomers(
        List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.CustomerId, p.Category, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalSpent = g.Sum(x => x.Revenue),
                FavoriteCategory = g.GroupBy(x => x.Category)
                                    .OrderByDescending(cg => cg.Sum(x => x.Quantity))
                                    .ThenBy(cg => cg.Key, StringComparer.Ordinal)
                                    .First()
                                    .Key
            })
            .OrderByDescending(x => x.TotalSpent)
            .ThenBy(x => x.CustomerId)
            .Take(5)
            .Join(customers,
                  x => x.CustomerId,
                  c => c.Id,
                  (x, c) => new TopCustomer(c.Name, c.City, x.TotalSpent, x.FavoriteCategory))
            .ToList();
    }
}