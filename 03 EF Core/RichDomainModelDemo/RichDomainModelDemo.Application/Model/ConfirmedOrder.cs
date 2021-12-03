namespace RichDomainModelDemo.Application.Model
{
    public class ConfirmedOrder : Order
    {
        public ConfirmedOrder(Order order, DateTime shippingDate, decimal shoppingCost) : base(order)
        {
            ShippingDate = shippingDate;
            ShoppingCost = shoppingCost;
        }
        protected ConfirmedOrder() { }               // For EF Core
        public DateTime ShippingDate { get; set; }
        public decimal ShoppingCost { get; set; }
    }
}
