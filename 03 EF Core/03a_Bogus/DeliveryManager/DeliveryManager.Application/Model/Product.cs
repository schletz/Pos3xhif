using System.Collections.Generic;

namespace DeliveryManager.Model;
    public class Product
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Product() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Product(string name, decimal price, Restaurant restaurant)
    {
        Name = name;
        Price = price;
        Restaurant = restaurant;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Restaurant Restaurant { get; set; }
    public List<OrderItem> OrderItems { get; } = new();
}