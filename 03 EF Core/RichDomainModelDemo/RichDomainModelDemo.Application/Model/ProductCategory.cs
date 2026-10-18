using System.ComponentModel.DataAnnotations.Schema;

namespace RichDomainModelDemo.Application.Model
{
    [Table("ProductCategory")]
    public class ProductCategory
    {
        public ProductCategory(string name)
        {
            Name = name;
        }
#pragma warning disable CS8618
        protected ProductCategory() { }
#pragma warning restore CS8618
        public int Id { get; private set; }
        public string Name { get; set; }
        public string? NameEn { get; set; }
    }
}
