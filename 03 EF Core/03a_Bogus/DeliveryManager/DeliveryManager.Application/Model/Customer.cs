using System.Collections.Generic;

namespace DeliveryManager.Model;
public class Customer
{
    public Customer(string phoneNumber, string email)
    {
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public List<Order> Orders { get; } = new();
}