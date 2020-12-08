using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Queries.Model
{
    public partial class Lesson
    {
        [Key]
        public long L_ID { get; set; }
        public long? L_Untis_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string L_Class { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string L_Teacher { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string L_Subject { get; set; }
        [Column(TypeName = "VARCHAR(8)")]
        public string L_Room { get; set; }
        public long? L_Day { get; set; }
        public long? L_Hour { get; set; }

        [ForeignKey(nameof(L_Class))]
        [InverseProperty(nameof(Schoolclass.Lesson))]
        [JsonIgnore]
        public virtual Schoolclass L_ClassNavigation { get; set; }
        [ForeignKey(nameof(L_Hour))]
        [InverseProperty(nameof(Period.Lesson))]
        [JsonIgnore]
        public virtual Period L_HourNavigation { get; set; }
        [ForeignKey(nameof(L_Teacher))]
        [InverseProperty(nameof(Teacher.Lesson))]
        [JsonIgnore]
        public virtual Teacher L_TeacherNavigation { get; set; }
    }
}
