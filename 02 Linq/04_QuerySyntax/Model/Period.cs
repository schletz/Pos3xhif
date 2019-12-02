using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Period
    {
        public Period()
        {
            Lessons = new HashSet<Lesson>();
            Tests = new HashSet<Test>();
        }

        public long P_Nr { get; set; }
        public DateTime P_From { get; set; }
        public DateTime P_To { get; set; }
        [JsonIgnore]
        public virtual ICollection<Lesson> Lessons { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
