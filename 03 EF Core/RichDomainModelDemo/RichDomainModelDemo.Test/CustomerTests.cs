using Microsoft.EntityFrameworkCore;
using RichDomainModelDemo.Application.Infrastructure;
using RichDomainModelDemo.Application.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RichDomainModelDemo.Test
{
    [Collection("Sequential")]

    public class CustomerTests
    {
        private StoreContext GetDbContext(bool deleteDB = false)
        {
            var opt = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=Stores.db")
                .UseLazyLoadingProxies()
                .LogTo((message) => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                .Options;
            if (deleteDB)
            {
                using (var db = new StoreContext(opt))
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            }
            return new StoreContext(opt);
        }
        [Fact]
        public void AddOrderSuccessTest()
        {
            using (var db = GetDbContext(deleteDB: true))
            {
                var customer = new Customer(firstname: "fn", lastname: "ln", address: new Address(Street: "street", Zip: "Zip", City: "City"));
                db.Customers.Add(customer);
                var order = new Order(
                    date: new DateTime(2021, 1, 1),
                    shippingAddress: new Address(Street: "street", Zip: "Zip", City: "City"),
                    customer: customer); customer.AddOrder(order);
                db.SaveChanges();
            }

            using (var db = GetDbContext())
            {
                Assert.True(db.Customers.Count() == 1);
                Assert.True(db.Customers.First().Orders.Count == 1);
            }
        }
        [Fact]
        public void ConfirmOrderSuccessTest()
        {
            using (var db = GetDbContext(deleteDB: true))
            {
                var customer = new Customer(firstname: "fn", lastname: "ln", address: new Address(Street: "street", Zip: "Zip", City: "City"));
                db.Customers.Add(customer);
                var order = new Order(
                    date: new DateTime(2021, 1, 1),
                    shippingAddress: new Address(Street: "street", Zip: "Zip", City: "City"),
                    customer: customer);
                customer.AddOrder(order);
                db.SaveChanges();
            }
            using (var db = GetDbContext())
            {
                var customer = db.Customers.First();
                customer.ConfirmOrder(customer.Orders.First());
                db.SaveChanges();
            }

            using (var db = GetDbContext())
            {
                var customer = db.Customers.Single();
                Assert.True(customer.Orders.OfType<ConfirmedOrder>().Count() == 1);
            }
        }
    }
}
