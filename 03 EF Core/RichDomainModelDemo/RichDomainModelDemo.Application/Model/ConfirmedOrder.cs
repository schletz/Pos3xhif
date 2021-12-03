namespace RichDomainModelDemo.Application.Model
{
    public class ConfirmedOrder : Order
    {
        public ConfirmedOrder(Order order, DateTime shippingDate, decimal shippingCost) : base(order)
        {
            ShippingDate = shippingDate;
            ShippingCost = shippingCost;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ConfirmedOrder() { }               // For EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime ShippingDate { get; set; }
        public decimal ShippingCost { get; set; }
        // Discriminator value. Needed for changing the type in an UPDATE statement.
        public override string OrderType { get; protected set; } = default!;

    }
}
