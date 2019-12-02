using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuerySyntax.Model
{
    public partial class Pupil
    {
        public long P_ID { get; set; }
        public string P_Account { get; set; }
        public string P_Lastname { get; set; }
        public string P_Firstname { get; set; }
        public string P_Class { get; set; }
        [JsonIgnore]
        public virtual Schoolclass P_ClassNavigation { get; set; }
    }
}
