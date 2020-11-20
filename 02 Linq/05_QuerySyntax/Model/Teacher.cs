using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Teacher
    {
        public Teacher()
        {
            Lessons = new HashSet<Lesson>();
            Schoolclasses = new HashSet<Schoolclass>();
            Tests = new HashSet<Test>();
        }

        public string T_ID { get; set; }
        public string T_Lastname { get; set; }
        public string T_Firstname { get; set; }
        public string T_Email { get; set; }
        public string T_Account { get; set; }
        [JsonIgnore]
        public virtual ICollection<Lesson> Lessons { get; set; }
        [JsonIgnore]
        public virtual ICollection<Schoolclass> Schoolclasses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
