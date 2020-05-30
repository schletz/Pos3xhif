using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace WahlfachAnmeldung.Models
{

    public class Subject
    {
        [Key]
        [MaxLength(8)]
        public string SubjectId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public List<Registration> Registrations { get; set; }
        public int Order { get; internal set; }
    }

    public class TeacherToken
    {
        [MaxLength(32)]
        public string TeacherTokenId { get; set; }
    }
}
