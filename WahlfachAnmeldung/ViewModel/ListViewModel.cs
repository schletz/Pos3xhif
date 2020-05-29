using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WahlfachAnmeldung.ViewModel
{
    public class ListViewModel
    {
        public class Registration
        {
            public string Schoolclass { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string Token { get; set; }
            public string Subject { get; set; }
        }
        public List<Registration> Registrations { get; set; }
        public List<RegistrationViewModel.RegistrationStat> RegistrationStats { get; set; }
    }
}
