using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryManager.Model;
public class Order
{
    [SetsRequiredMembers]
    public Order(Customer customer, Restaurant restaurant, DateTime orderDate)
    {
        Customer = customer;
        Restaurant = restaurant;
        OrderDate = orderDate;
    }

    // Used by EF Core to materialize entities. The required members prevent uninitialized instances.
    private Order() { }

    public int Id { get; set; }
    public required Customer Customer { get; set; }
    public required Restaurant Restaurant { get; set; }
    public Driver? Driver { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<OrderItem> OrderItems { get; } = new();
}
