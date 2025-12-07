using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Queries.Model
{
    public partial class Schoolclass
    {
        public Schoolclass()
        {
            Lesson = new HashSet<Lesson>();
            Pupil = new HashSet<Pupil>();
            Test = new HashSet<Test>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(8)")]
        public string C_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string C_Department { get; set; }
        [Column(TypeName = "VARCHAR(8)")]
        public string C_ClassTeacher { get; set; }

        [ForeignKey(nameof(C_ClassTeacher))]
        [InverseProperty(nameof(Teacher.Schoolclass))]
        [JsonIgnore]
        public virtual Teacher C_ClassTeacherNavigation { get; set; }
        [InverseProperty("L_ClassNavigation")]
        [JsonIgnore]
        public virtual ICollection<Lesson> Lesson { get; set; }
        [InverseProperty("P_ClassNavigation")]
        [JsonIgnore]
        public virtual ICollection<Pupil> Pupil { get; set; }
        [InverseProperty("TE_ClassNavigation")]
        [JsonIgnore]
        public virtual ICollection<Test> Test { get; set; }
    }
}
