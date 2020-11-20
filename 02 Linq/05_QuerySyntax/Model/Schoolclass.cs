using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Schoolclass
    {
        public Schoolclass()
        {
            Lessons = new HashSet<Lesson>();
            Pupils = new HashSet<Pupil>();
            Tests = new HashSet<Test>();
        }

        public string C_ID { get; set; }
        public string C_Department { get; set; }
        public string C_ClassTeacher { get; set; }
        [JsonIgnore]
        public virtual Teacher C_ClassTeacherNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<Lesson> Lessons { get; set; }
        [JsonIgnore]
        public virtual ICollection<Pupil> Pupils { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
