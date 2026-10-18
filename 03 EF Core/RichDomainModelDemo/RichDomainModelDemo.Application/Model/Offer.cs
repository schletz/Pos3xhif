using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{

    [Table("Offer")]
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
#pragma warning disable CS8618
        protected Offer() { }
#pragma warning restore CS8618
        public int Id { get; private set; }
        public int ProductEan { get; set; }
        public virtual Product Product { get; set; }  // virtual for EF Core lazy loading
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }      // virtual for EF Core lazy loading
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
