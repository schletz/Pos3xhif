using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Grouping.Model
{
    public class Period
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Nr { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        [JsonIgnore]
        public List<Lesson> Lessons { get; } = new();
        [JsonIgnore]
        public List<Exam> Exams { get; } = new();
    }
}
