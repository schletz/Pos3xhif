using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Test
    {
        public long TE_ID { get; set; }
        public string TE_Class { get; set; }
        public string TE_Teacher { get; set; }
        public string TE_Subject { get; set; }
        public DateTime TE_Date { get; set; }
        public long TE_Lesson { get; set; }
        [JsonIgnore]
        public virtual Schoolclass TE_ClassNavigation { get; set; }
        [JsonIgnore]
        public virtual Period TE_LessonNavigation { get; set; }
        [JsonIgnore]
        public virtual Teacher TE_TeacherNavigation { get; set; }
    }
}
