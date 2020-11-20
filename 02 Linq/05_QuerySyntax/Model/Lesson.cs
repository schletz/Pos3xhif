using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Lesson
    {
        public long L_ID { get; set; }
        public long? L_Untis_ID { get; set; }
        public string L_Class { get; set; }
        public string L_Teacher { get; set; }
        public string L_Subject { get; set; }
        public string L_Room { get; set; }
        public long? L_Day { get; set; }
        public long? L_Hour { get; set; }
        [JsonIgnore]
        public virtual Schoolclass L_ClassNavigation { get; set; }
        [JsonIgnore]
        public virtual Period L_HourNavigation { get; set; }
        [JsonIgnore]
        public virtual Teacher L_TeacherNavigation { get; set; }
    }
}
