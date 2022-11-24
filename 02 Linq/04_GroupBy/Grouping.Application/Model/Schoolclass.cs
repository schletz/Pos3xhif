using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Grouping.Model
{
    public class Schoolclass
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Schoolclass() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Key]
        [MaxLength(8)]
        public string Id { get; set; }
        public string Department { get; set; }
        [CsvHelper.Configuration.Attributes.Default(null)]
        [MaxLength(8)]
        public string? ClassTeacherId { get; set; }
        [JsonIgnore]
        [CsvHelper.Configuration.Attributes.Ignore]
        public Teacher? ClassTeacher { get; set; }
        [JsonIgnore]
        public List<Lesson> Lessons { get; } = new();
        [JsonIgnore]
        public List<Pupil> Pupils { get; } = new();
        [JsonIgnore]
        public List<Exam> Exams { get; } = new();
    }
}
