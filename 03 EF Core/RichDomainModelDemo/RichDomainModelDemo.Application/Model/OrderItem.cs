using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("OrderItem")]
    public class OrderItem
    {
        public OrderItem(Offer offer, decimal price, int quantity)
        {
            OfferId = offer.Id;
            Offer = offer;
            Price = price;
            Quantity = quantity;
            Order = default!;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OrderItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public int OfferId { get; set; }
        public virtual Offer Offer { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; private set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
