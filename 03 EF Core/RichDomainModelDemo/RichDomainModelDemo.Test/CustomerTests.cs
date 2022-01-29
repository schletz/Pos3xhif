using RichDomainModelDemo.Application.Model;
using System;
using System.Linq;
using Xunit;

namespace RichDomainModelDemo.Test
{
    public class CustomerTests : DatabaseTest
    {
        /// <summary>
        /// Constructor. Called before each test.
        /// </summary>
        public CustomerTests()
        {
            _db.Database.EnsureCreated();
            var customer = new Customer(firstname: "fn", lastname: "ln", address: new Address(Street: "street", Zip: "Zip", City: "City"));
            _db.Customers.Add(customer);
            var order = new Order(
                date: new DateTime(2021, 1, 1),
                shippingAddress: new Address(Street: "street", Zip: "Zip", City: "City"));
            customer.AddOrder(order);
            _db.SaveChanges();
        }
        [Fact]
        public void AddOrderSuccessTest()
        {
            // ToList produces a SELECT * FROM Customers query. If we write only
            // .Count() we are producing a SELCT COUNT(*) FROM Customers query,
            // so we cannot test the correct mapping.
            Assert.True(_db.Customers.ToList().Count() == 1);
            Assert.True(_db.Customers.ToList().First().Orders.Count == 1);
        }
        [Fact]
        public void ConfirmOrderSuccessTest()
        {
            var customer = _db.Customers.First();
            customer.ConfirmOrder(customer.Orders.First());
            _db.SaveChanges();

            // Clear navigations and objects in memory
            _db.ChangeTracker.Clear();
            // Re-read customer, because the existing customer has an order in memory
            customer = _db.Customers.First();
            Assert.True(customer.Orders.OfType<ConfirmedOrder>().Count() == 1);
        }
    }
}
