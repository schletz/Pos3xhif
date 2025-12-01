namespace DeliveryManager.Model;

public class Address
{
    public Address(string street, string zip, string city)
    {
        Street = street;
        Zip = zip;
        City = city;
    }

    public string Street { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
}