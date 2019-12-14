using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Crud.Model
{
    public partial class Period
    {
        public Period()
        {
            Lesson = new HashSet<Lesson>();
            Test = new HashSet<Test>();
        }

        [Key]
        public long P_Nr { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime P_From { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime P_To { get; set; }

        [InverseProperty("L_HourNavigation")]
        [JsonIgnore]
        public virtual ICollection<Lesson> Lesson { get; set; }
        [InverseProperty("TE_LessonNavigation")]
        [JsonIgnore]
        public virtual ICollection<Test> Test { get; set; }
    }
}
