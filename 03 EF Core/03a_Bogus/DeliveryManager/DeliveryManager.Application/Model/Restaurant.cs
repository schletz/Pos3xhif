using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryManager.Model;
public class Restaurant
{
    [SetsRequiredMembers]
    public Restaurant(string name, Address address)
    {
        Name = name;
        Address = address;
    }

    // Used by EF Core to materialize entities. The required members prevent uninitialized instances.
    private Restaurant() { }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required Address Address { get; set; }
    public List<Product> Products { get; } = new();
    public List<Order> Orders { get; } = new();
}
