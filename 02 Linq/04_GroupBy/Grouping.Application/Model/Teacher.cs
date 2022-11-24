using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grouping.Model
{
    public class Teacher
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Teacher() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Key]
        [MaxLength(8)]
        public string Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Account { get; set; }
        [JsonIgnore]
        public List<Lesson> Lessons { get; }
        [JsonIgnore]
        public List<Schoolclass> Schoolclasses { get; }
        [JsonIgnore]
        public List<Exam> Exams { get; }
    }
}
