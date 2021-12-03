using System.ComponentModel.DataAnnotations;

namespace RichDomainModelDemo.Application.Model
{
    /// <summary>
    /// Value Object for address data.
    /// For Value objects we use the new record feature of C# 9.
    /// Value objects has to be configured in StoreContext.OnModelCreating() with OwnsOne()
    /// </summary>
    public record Address([MaxLength(255)] string Street, [MaxLength(255)] string Zip, [MaxLength(255)] string City);
}
