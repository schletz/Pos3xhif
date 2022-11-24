using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Grouping.Model
{
    public class Pupil
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Pupil() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Account { get; set; }
        [MaxLength(8)]
        public string SchoolclassId { get; set; }
        [JsonIgnore]
        [CsvHelper.Configuration.Attributes.Ignore]
        public Schoolclass Schoolclass { get; set; }
    }
}
