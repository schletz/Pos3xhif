using RichDomainModelDemo.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RichDomainModelDemo.Test
{
    public class OrderTests
    {
        private void PrepareDatabase()
        {
            using (var db = TestHelpers.GetDbContext(deleteDB: true))
            {
                var product = new Product(ean: 1001, name: "name", productCategory: new ProductCategory(name: "name"));
                db.Products.Add(product);

                var store = new Store(name: "name");
                db.Stores.Add(store);

                var customer = new Customer(
                        firstname: "firstname",
                        lastname: "lastname",
                        address: new Address(Street: "Street", Zip: "Zip", City: "City"));
                db.Customers.Add(customer);

                var offer = new Offer(
                    product: product,
                    store: store,
                    price: 1000,
                    lastUpdate: new DateTime(2021, 2, 1));
                db.Offers.Add(offer);

                var order = new Order(
                    date: new DateTime(2021, 1, 1),
                    shippingAddress: new Address(Street: "Street", Zip: "Zip", City: "City"));
                customer.AddOrder(order);
                db.SaveChanges();
            }
        }
        [Fact]
        public void AddOrderItemNewOfferSuccessTest()
        {
            PrepareDatabase();
            using (var db = TestHelpers.GetDbContext())
            {
                var order = db.Orders.First();
                var orderItem = new OrderItem(
                    offer: db.Offers.First(),
                    price: 950, quantity: 1);
                order.AddOrderItem(orderItem);
                db.SaveChanges();
            }
            using (var db = TestHelpers.GetDbContext())
            {
                var order = db.Orders.First();
                Assert.True(order.OrderItems.Count == 1);
            }
        }
        [Fact]
        public void AddOrderItemExistingOfferSuccessTest()
        {
            PrepareDatabase();
            using (var db = TestHelpers.GetDbContext())
            {
                var order = db.Orders.First();
                var orderItem = new OrderItem(
                    offer: db.Offers.First(),
                    price: 950, quantity: 1);
                order.AddOrderItem(orderItem);
                db.SaveChanges();
            }
            using (var db = TestHelpers.GetDbContext())
            {
                var order = db.Orders.First();
                var orderItem = new OrderItem(
                    offer: db.Offers.First(),
                    price: 950, quantity: 2);
                order.AddOrderItem(orderItem);
                db.SaveChanges();
            }
            using (var db = TestHelpers.GetDbContext())
            {
                var order = db.Orders.First();
                Assert.True(order.OrderItems.First().Quantity == 3);
            }

        }
    }
}
