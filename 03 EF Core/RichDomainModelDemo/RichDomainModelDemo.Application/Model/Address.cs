using System.ComponentModel.DataAnnotations;

namespace RichDomainModelDemo.Application.Model
{
    public record Address([MaxLength(255)] string Street, [MaxLength(255)] string Zip, [MaxLength(255)] string City);
}
