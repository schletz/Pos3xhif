using System.ComponentModel.DataAnnotations;

namespace WahlfachAnmeldung.Models
{
    public class TeacherToken
    {
        [MaxLength(32)]
        public string TeacherTokenId { get; set; }
    }
}
