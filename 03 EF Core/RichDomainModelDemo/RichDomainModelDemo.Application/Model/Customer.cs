using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Customer")]
    public class Customer
    {
        /// <summary>
        /// The customer is an aggregate for its orders.
        /// </summary>
        public Customer(string firstname, string lastname, Address address)
        {
            Firstname = firstname;
            Lastname = lastname;
            Address = address;
        }
#pragma warning disable CS8618
        protected Customer() { }
#pragma warning restore CS8618

        public int Id { get; private set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Address Address { get; set; }  // Value object. Use Entity<Customer>().OwnsOne(c=>c.Address) for configuring in DbContext.

        // Backing field for the Orders navigation property. Prevents adding orders from the outside.
        // Convention for backing field in EF Core: _ + property name
        protected List<Order> _orders = new();
        public virtual IReadOnlyCollection<Order> Orders => _orders;  // Expose orders as ReadOnlyCollection to prevent modifications.

        /// <summary>
        /// Calculate shipping date and shipping cost. Converts an existing order to a confirmed order.
        /// </summary>
        public void ConfirmOrder(Order order)
        {
            if (order is ConfirmedOrder) { return; }
            var confirmedOrder = new ConfirmedOrder(order, order.Date.AddDays(3), 10);
            // Remove() and Add() produces an UPDATE statement. Does not work without a discriminator property in Order!
            _orders.Remove(order);
            _orders.Add(confirmedOrder);
        }
        public void AddOrder(Order order)
        {
            _orders.Add(order);
        }
    }
}
