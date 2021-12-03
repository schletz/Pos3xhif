using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Order")]
    public class Order
    {
        protected List<OrderItem> _orderItems = new();

        public Order(DateTime date, Address shippingAddress, Customer customer)
        {
            Date = date;
            ShippingAddress = shippingAddress;
            Customer = customer;
            CustomerId = customer.Id;
        }

        public Order(Order order)
        {
            Id = order.Id;
            Date = order.Date;
            // Deep copy (cloning) of ShippingAddress because the other instance may be tracked by EF Core.
            ShippingAddress = order.ShippingAddress with { };
            CustomerId = order.CustomerId;
            Customer = order.Customer;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Order() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public DateTime Date { get; set; }
        public Address ShippingAddress { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
        // Discriminator value. Needed for changing the type in an UPDATE statement.
        public virtual string OrderType { get; protected set; } = default!;
        public void AddOrderItem(OrderItem orderItem)
        {
            // DO NOT query _orderItems because lazy loading is bound to OrderItems
            var itemDb = OrderItems.FirstOrDefault(o => o.OfferId == orderItem.OfferId && o.Price == orderItem.Price);
            if (itemDb is not null)
            {
                itemDb.Quantity += orderItem.Quantity;
                return;
            }
            _orderItems.Add(orderItem);
        }

    }
}
