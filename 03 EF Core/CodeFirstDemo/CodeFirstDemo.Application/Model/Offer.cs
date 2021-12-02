namespace CodeFirstDemo.Application.Model
{
    public class Offer
    {
        public Offer(Product product, Store store, decimal price, DateTime lastUpdate)
        {
            Product = product;
            ProductEan = product.Ean;
            Store = store;
            StoreId = store.Id;
            Price = price;
            LastUpdate = lastUpdate;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Offer() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public int ProductEan { get; set; }
        public Product Product { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
