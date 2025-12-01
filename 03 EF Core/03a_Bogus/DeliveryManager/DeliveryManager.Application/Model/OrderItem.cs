using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryManager.Model;
public class OrderItem
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected OrderItem() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public OrderItem(Order order, Product product, int quantity)
    {
        Order = order;
        Product = product;
        Quantity = quantity;
    }
    public int Id { get; set; }
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    public int Quantity { get; set; }
}