using System;
using System.Collections.Generic;

namespace Eventmanager.Model
{
    public class Guest
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Guest()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Guest(string firstname, string lastname, DateOnly birthDate)
        {
            Firstname = firstname;
            Lastname = lastname;
            BirthDate = birthDate;
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateOnly BirthDate { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Ticket> Tickets { get; set; } = new();
    }
}