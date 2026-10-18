using System.Collections.Generic;

namespace DeliveryManager.Model;
public class Restaurant
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Restaurant() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Restaurant(string name, Address address)
    {
        Name = name;
        Address = address;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<Product> Products { get; } = new();
    public List<Order> Orders { get; } = new();
}
