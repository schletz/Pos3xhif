using System.ComponentModel.DataAnnotations;

namespace WahlfachAnmeldung.Models
{
    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }
        [Required]
        public Token Token { get; set; }
        [Required]
        public Subject Subject { get; set; }
        public int Rating { get; set; }
    }
}
