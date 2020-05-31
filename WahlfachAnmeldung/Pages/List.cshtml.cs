using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WahlfachAnmeldung.Models;

namespace WahlfachAnmeldung.Pages
{
    /// <summary>
    /// Stellt die eingetragenen Erstreihungen als Tabelle dar.
    /// </summary>
    public class ListModel : PageModel
    {
        private readonly RegistrationContext _context;
        public TeacherToken Token { get; private set; } = new TeacherToken();

        public ListModel(RegistrationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Daten für die Tabelle der eingetragenen Registrierungen.
        /// </summary>
        public class Registration
        {
            public string Schoolclass { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string Token { get; set; }
            public string Subject { get; set; }
        }

        /// <summary>
        /// Daten für die Statistik der Registrierungen.
        /// </summary>
        public class RegistrationStat
        {
            public string SubjectId { get; set; }
            public int RegistrationCount { get; set; }
        }

        // Bindingfelder für die View.
        public List<Registration> Registrations { get; private set; }
        public List<RegistrationStat> RegistrationStats { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if ((Token = _context.TeacherTokens.Find(id)) == null) { return NotFound(); }
            await InitializeAsync();
            return Page();
        }

        /// <summary>
        /// Liest die benötigten Werte für die Anzeige der Seite aus der Datenbank.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            RegistrationStats = await (from r in _context.Registrations
                                     where r.Rating == 1
                                     group r by r.Subject.SubjectId into g
                                     orderby g.Key
                                     select new RegistrationStat
                                     {
                                         SubjectId = g.Key,
                                         RegistrationCount = g.Count()
                                     }).ToListAsync();

            Registrations = await (from r in _context.Registrations
                                 where r.Rating == 1
                                 orderby r.Token.SchoolClass, r.Token.LastValidRegistration
                                 select new Registration
                                 {
                                     Schoolclass = r.Token.SchoolClass,
                                     RegistrationDate = r.Token.LastValidRegistration ?? DateTime.MinValue,
                                     Token = r.Token.TokenId,
                                     Subject = r.Subject.SubjectId
                                 }).ToListAsync();

        }
    }
}