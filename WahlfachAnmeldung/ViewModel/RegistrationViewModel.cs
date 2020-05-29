using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WahlfachAnmeldung.Models;

namespace WahlfachAnmeldung.ViewModel
{
    public class RegistrationViewModel : IValidatableObject
    {
        public class SubjectRegistration
        {
            public string SubjectId { get; set; }
            public string SubjectName { get; set; }
            public int RegistrationId { get; set; }
            public int Rating { get; set; }
        }

        public class RegistrationStat
        {
            public string SubjectId { get; set; }
            public int RegistrationCount { get; set; }
        }

        public string Token { get; set; }
        public DateTime? LastValidRegistration { get; set; }
        public List<RegistrationStat> RegistrationStats { get; set; }
        public List<SubjectRegistration> SubjectRegistrations { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var rating = SubjectRegistrations.Select(s => s.Rating);
            var count = SubjectRegistrations.Count;

            if (!(rating.Min() == 1 && rating.Max() == count && rating.Distinct().Count() == count))
            {
                yield return new ValidationResult("Ungültige Auswahl");
            }
        }
    }


}
