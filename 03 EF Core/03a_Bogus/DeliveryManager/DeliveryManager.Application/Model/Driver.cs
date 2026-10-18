using System.Collections.Generic;

namespace DeliveryManager.Model;
public class Driver
{
    public Driver(string firstname, string lastname)
    {
        Firstname = firstname;
        Lastname = lastname;
    }

    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }

    public List<Order> Orders { get; } = new();
}