using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Order")]
    public class Order
    {
        protected List<OrderItem> _orderItems = new ();

        public Order(DateTime date, Address shippingAddress)
        {
            Date = date;
            ShippingAddress = shippingAddress;
        }

        public Order(Order order)
        {
            Id = order.Id;
            Date = order.Date;
            ShippingAddress = order.ShippingAddress;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Order() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public DateTime Date { get; set; }
        public Address ShippingAddress { get; set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    }
}
