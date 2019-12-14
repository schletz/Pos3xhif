using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Queries.Model
{
    public partial class Teacher
    {
        public Teacher()
        {
            Lesson = new HashSet<Lesson>();
            Schoolclass = new HashSet<Schoolclass>();
            Test = new HashSet<Test>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(8)")]
        public string T_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string T_Lastname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string T_Firstname { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string T_Email { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string T_Account { get; set; }

        [InverseProperty("L_TeacherNavigation")]
        [JsonIgnore]
        public virtual ICollection<Lesson> Lesson { get; set; }
        [InverseProperty("C_ClassTeacherNavigation")]
        [JsonIgnore]
        public virtual ICollection<Schoolclass> Schoolclass { get; set; }
        [InverseProperty("TE_TeacherNavigation")]
        [JsonIgnore]
        public virtual ICollection<Test> Test { get; set; }
    }
}
