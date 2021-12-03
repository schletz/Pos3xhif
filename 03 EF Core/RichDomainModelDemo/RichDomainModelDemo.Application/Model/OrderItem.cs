using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("OrderItem")]
    public class OrderItem
    {
        public OrderItem(Offer offer, Order order, decimal price, int quantity)
        {
            OfferId = offer.Id;
            Offer = offer;
            OrderId = order.Id;
            Order = order;
            Price = price;
            Quantity = quantity;
        }

        public int Id { get; private set; }
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
