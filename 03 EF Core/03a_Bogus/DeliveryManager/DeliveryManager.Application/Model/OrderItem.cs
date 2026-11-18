using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryManager.Model;
public class OrderItem
{
    [SetsRequiredMembers]
    public OrderItem(Order order, Product product, int quantity)
    {
        Order = order;
        Product = product;
        Quantity = quantity;
    }

    // Used by EF Core to materialize entities. The required members prevent uninitialized instances.
    private OrderItem() { }

    public int Id { get; set; }
    [ForeignKey("OrderId")]
    public required Order Order { get; set; }
    [ForeignKey("ProductId")]
    public required Product Product { get; set; }
    public int Quantity { get; set; }
}
