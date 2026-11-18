using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryManager.Model;
public class Product
{
    [SetsRequiredMembers]
    public Product(string name, decimal price, Restaurant restaurant)
    {
        Name = name;
        Price = price;
        Restaurant = restaurant;
    }

    // Used by EF Core to materialize entities. The required members prevent uninitialized instances.
    private Product() { }

    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    public required Restaurant Restaurant { get; set; }
    public List<OrderItem> OrderItems { get; } = new();
}
