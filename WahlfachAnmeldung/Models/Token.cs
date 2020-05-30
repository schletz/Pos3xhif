using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WahlfachAnmeldung.Models
{
    public class Token
    {
        [Key]
        [MaxLength(32)]
        public string TokenId { get; set; }
        [Required]
        [MaxLength(8)]
        public string SchoolClass { get; set; }
        public Subject AssignedSubject { get; set; }
        public List<Registration> Registrations { get; set; }
        public DateTime? LastValidRegistration { get; set; }
    }
}
