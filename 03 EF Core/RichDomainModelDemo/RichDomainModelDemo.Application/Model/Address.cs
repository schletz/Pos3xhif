using System.ComponentModel.DataAnnotations;

namespace RichDomainModelDemo.Application.Model
{
    /// <summary>
    /// Value object for adress information.
    /// </summary>
    public record Address([MaxLength] string Street, [MaxLength] string Zip, [MaxLength] string City);
}
