using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Order")]
    public class Order
    {
        /// <summary>
        /// Customer is the aggregate for Order, so it is not allowed to set any customer.
        /// An order has to be placed by Customer.AddOrder().
        /// </summary>
        public Order(DateTime date, Address shippingAddress)
        {
            Date = date;
            ShippingAddress = shippingAddress;
            // Navigation set by EF Core.
            Customer = default!;
        }
        /// <summary>
        /// Copyconstructor for changing the state from order to confirmed order.
        /// </summary>
        /// <param name="order"></param>
        protected Order(Order order)
        {
            Id = order.Id;
            Date = order.Date;
            // Deep copy (cloning) of ShippingAddress. Without cloning EF Core will throw an exception (values are not set).
            ShippingAddress = order.ShippingAddress with { };
            CustomerId = order.CustomerId;
            Customer = order.Customer;
        }
#pragma warning disable CS8618 
        protected Order() { }
#pragma warning restore CS8618

        public int Id { get; private set; }
        public DateTime Date { get; set; }
        public Address ShippingAddress { get; set; }
        public int CustomerId { get; private set; }             // private set because order is managed by the Customer object.
        public virtual Customer Customer { get; private set; }  // private set because order is managed by the Customer object.

        // Backing field for the OrderItems navigation property. Prevents adding order items from the outside.
        // Convention for backing field in EF Core: _ + property name
        protected List<OrderItem> _orderItems = new();
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