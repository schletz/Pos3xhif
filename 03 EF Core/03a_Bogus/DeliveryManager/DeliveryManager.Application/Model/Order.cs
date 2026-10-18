using System;
using System.Collections.Generic;

namespace DeliveryManager.Model;
    public class Order
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Order() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Order(Customer customer, Restaurant restaurant, DateTime orderDate)
    {
        Customer = customer;
        Restaurant = restaurant;
        OrderDate = orderDate;
    }

    public int Id { get; set; }
    public Customer Customer { get; set; }
    public Restaurant Restaurant { get; set; }
    public Driver? Driver { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<OrderItem> OrderItems { get; } = new();
}