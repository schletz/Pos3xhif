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
        public TeacherToken Token { get; private set; } = new TeacherToken();
        public List<Registration> Registrations { get; private set; } = new List<Registration>();
        public List<Token> MissingRegistrations { get; private set; } = new List<Token>();
        public List<RegistrationStat> RegistrationStats { get; private set; } = new List<RegistrationStat>();

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
                                       group r by r.SubjectId into g
                                       orderby g.Key
                                       select new RegistrationStat
                                       {
                                           SubjectId = g.Key,
                                           RegistrationCount = g.Count()
                                       }).ToListAsync();
            int tokenCount = _context.Tokens.Count();
            int totalRegistrations = RegistrationStats.Sum(r => r.RegistrationCount);
            RegistrationStats.Add(new RegistrationStat
            {
                SubjectId = "Summe",
                RegistrationCount = totalRegistrations
            });
            RegistrationStats.Add(new RegistrationStat
            {
                SubjectId = "Fehlende",
                RegistrationCount = tokenCount - totalRegistrations
            });

            Registrations = await (from r in _context.Registrations
                                   where r.Rating == 1
                                   orderby r.Token.SchoolClass, r.Token.LastValidRegistration
                                   select new Registration
                                   {
                                       Schoolclass = r.Token.SchoolClass,
                                       RegistrationDate = r.Token.LastValidRegistration ?? DateTime.MinValue,
                                       Token = r.TokenId,
                                       Subject = r.SubjectId
                                   }).ToListAsync();

            MissingRegistrations = await (from t in _context.Tokens
                                          orderby t.SchoolClass
                                          where !t.Registrations.Any()
                                          select t).ToListAsync();

        }
    }
}