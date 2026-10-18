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
        public decimal TotalPrice => OrderItems.Sum(o => o.Price) + ShippingCost;
    }
}
