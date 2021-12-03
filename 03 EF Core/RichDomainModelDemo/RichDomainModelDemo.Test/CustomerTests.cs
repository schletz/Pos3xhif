using RichDomainModelDemo.Application.Model;
using System;
using System.Linq;
using Xunit;

namespace RichDomainModelDemo.Test
{
    [Collection("Sequential")]

    public class CustomerTests
    {
        private void PrepareDatabase()
        {
            using (var db = TestHelpers.GetDbContext(deleteDB: true))
            {
                var customer = new Customer(firstname: "fn", lastname: "ln", address: new Address(Street: "street", Zip: "Zip", City: "City"));
                db.Customers.Add(customer);
                var order = new Order(
                    date: new DateTime(2021, 1, 1),
                    shippingAddress: new Address(Street: "street", Zip: "Zip", City: "City"),
                    customer: customer); customer.AddOrder(order);
                db.SaveChanges();
            }
        }
        [Fact]
        public void AddOrderSuccessTest()
        {
            PrepareDatabase();
            using (var db = TestHelpers.GetDbContext())
            {
                Assert.True(db.Customers.Count() == 1);
                Assert.True(db.Customers.First().Orders.Count == 1);
            }
        }
        [Fact]
        public void ConfirmOrderSuccessTest()
        {
            PrepareDatabase();
            using (var db = TestHelpers.GetDbContext())
            {
                var customer = db.Customers.First();
                customer.ConfirmOrder(customer.Orders.First());
                db.SaveChanges();
            }

            using (var db = TestHelpers.GetDbContext())
            {
                var customer = db.Customers.First();
                Assert.True(customer.Orders.OfType<ConfirmedOrder>().Count() == 1);
            }
        }
    }
}
