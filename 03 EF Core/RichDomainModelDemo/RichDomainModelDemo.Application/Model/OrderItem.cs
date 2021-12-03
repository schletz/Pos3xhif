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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OrderItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public int OfferId { get; set; }
        public virtual Offer Offer { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
