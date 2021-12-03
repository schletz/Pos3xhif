using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("Customer")]
    public class Customer
    {
        public Customer(string firstname, string lastname, Address address)
        {
            Firstname = firstname;
            Lastname = lastname;
            Address = address;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Customer() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected List<Order> _orders = new();  // Convention for backing field in EF Core: _ + property name
        public int Id { get; private set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Address Address { get; set; }  // Value object. Use Entity<Customer>().OwnsOne(c=>c.Address) for configuring in DbContext.
        public virtual IReadOnlyCollection<Order> Orders => _orders;  // Prevent adding data from outside.
        public void ConfirmOrder(Order order)
        {
            if (order is ConfirmedOrder) { return; }
            var confirmedOrder = new ConfirmedOrder(order, order.Date.AddDays(3), 10);
            _orders.Remove(order);
            _orders.Add(confirmedOrder);
        }
        public void AddOrder(Order order)
        {
            _orders.Add(order);
        }
    }
}
