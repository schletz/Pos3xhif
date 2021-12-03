namespace RichDomainModelDemo.Application.Model
{
    public class ConfirmedOrder : Order
    {
        /// <summary>
        /// An order that has the state "confirmed". Inherited from order.
        /// </summary>
        public ConfirmedOrder(Order order, DateTime shippingDate, decimal shippingCost) : base(order)
        {
            ShippingDate = shippingDate;
            ShippingCost = shippingCost;
        }
#pragma warning disable CS8618
        protected ConfirmedOrder() { }               // For EF Core
#pragma warning restore CS8618
        public DateTime ShippingDate { get; set; }
        public decimal ShippingCost { get; set; }
        // Discriminator value. Needed for changing the type in an UPDATE statement.
        // Has to be configured in OnModelCreating() with HasDiscriminator()
        public override string OrderType { get; protected set; } = default!;
        public decimal TotalPrice => OrderItems.Sum(o => o.Price) + ShippingCost;
    }
}
