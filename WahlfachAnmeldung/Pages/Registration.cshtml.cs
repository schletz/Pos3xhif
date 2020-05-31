using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WahlfachAnmeldung.Models;

namespace WahlfachAnmeldung.Pages
{
    /// <summary>
    /// Stellt das HTML Formular für die Reihung sowie die Statistik dar.
    /// </summary>
    public class RegistrationModel : PageModel
    {
        /// <summary>
        /// Daten für eine Reihung.
        /// </summary>
        public class SubjectRegistration
        {
            public string SubjectId { get; set; }
            public string SubjectName { get; set; }
            public int RegistrationId { get; set; }
            public int Rating { get; set; }
        }

        /// <summary>
        /// Daten für die Statistik.
        /// </summary>
        public class RegistrationStat
        {
            public string SubjectId { get; set; }
            public int RegistrationCount { get; set; }
        }

        /// <summary>
        /// Datentyp für die Anzeige des Formulares. Ist ein eigener Typ, damit er mit
        /// IValidatableObject geprüft werden kann.
        /// </summary>
        public class SubjectRegistrationList : List<SubjectRegistration>, IValidatableObject
        {
            public SubjectRegistrationList() : base() { }
            public SubjectRegistrationList(IEnumerable<SubjectRegistration> collection) : base(collection) { }

            /// <summary>
            /// Prüft auf doppelte Zahlenwerte in der Reihung.
            /// </summary>
            /// <param name="validationContext"></param>
            /// <returns></returns>
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                DateTime now = DateTime.UtcNow;
                // Nach der Deadline ist keine Anmeldung mehr möglich.
                if (now >= RegistrationClosedFrom)
                {
                    yield return new ValidationResult("Der Anmeldezeitraum ist beendet.");
                    yield break;
                }
                var ratingStats = new
                {
                    Min = this.Min(r => r.Rating),
                    Max = this.Max(r => r.Rating),
                    Count = this.Count,
                    DistinctCount = this.Select(r => r.Rating).Distinct().Count()
                };

                if (!(ratingStats.Min == 1 && ratingStats.Max == ratingStats.Count && ratingStats.DistinctCount == ratingStats.Count))
                {
                    yield return new ValidationResult("Ungültige Auswahl");
                    yield break;
                }

                // Im eingeschränkten Zeitraum können nur mehr Fächer gebucht werden, die unter der maximalen
                // Anmeldezahl haben.
                if (now >= RegistrationRestrictedFrom)
                {
                    var firstRating = this.SingleOrDefault(r => r.Rating == 1);
                    var context = validationContext.GetService(typeof(RegistrationContext)) as RegistrationContext;
                    if (context.Registrations.Count(r => r.Rating == 1 && r.SubjectId == firstRating.SubjectId) >= MaxRegistrations)
                    {
                        yield return new ValidationResult($"Das Fach {firstRating.SubjectId} hat bereits {MaxRegistrations} Erstreihungen. Die Reihung wurde daher nicht eingetragen oder verändert.");
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Maximale Zahl der Erstreihungen, die im Zeitraum ab RegistrationRestrictedFrom gebucht
        /// werden können.
        /// </summary>
        public static int MaxRegistrations { get; } = 30;
        /// <summary>
        /// Ab diesem Zeitpunkt kann die Reihung nur soweit verändert/eingegbeben werden, sodass
        /// MaxRegistrations nicht überschritten wird.
        /// </summary>
        public static DateTime RegistrationRestrictedFrom { get; } = new DateTime(2020, 6, 7, 22, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Ab diesem Zeitpunkt sind keine Änderungen oder Eingaben mehr möglich.
        /// </summary>
        public static DateTime RegistrationClosedFrom { get; } = new DateTime(2020, 6, 11, 22, 0, 0, DateTimeKind.Utc);

        // Binding für die Anzeige des Datums der letzten Änderung.
        public Token Token { get; private set; }

        // Binding für die Statistik.
        public List<RegistrationStat> RegistrationStats { get; private set; } = new List<RegistrationStat>();
        // Bereits eingetragene Reihungen in der Datenbank.
        public SubjectRegistrationList ExistingSubjectRegistrations { get; private set; } = new SubjectRegistrationList();
        // Binding für das Formular
        [BindProperty]
        public SubjectRegistrationList SubjectRegistrations { get; set; } = new SubjectRegistrationList();

        private readonly ILogger<RegistrationModel> _logger;
        private readonly RegistrationContext _context;

        public RegistrationModel(ILogger<RegistrationModel> logger, RegistrationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = _context.Tokens.Find(id);
            if (token == null) { return NotFound(); }

            await InitializeAsync(token);
            // Initialisierung des Formulares mit eingetragenen Reihungen (wenn vorhanden)
            SubjectRegistrations = ExistingSubjectRegistrations;
            return Page();
        }

        /// <summary>
        /// Speichert die Formulareingaben in der Datenbank.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string id)
        {
            // Nur mit gültigem Token verarbeiten.
            var token = _context.Tokens.Find(id);
            if (token == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                foreach (var r in SubjectRegistrations ?? Enumerable.Empty<SubjectRegistration>())
                {
                    if (r.RegistrationId == 0)
                    {
                        _context.Registrations.Add(new Registration
                        {
                            SubjectId = r.SubjectId,
                            TokenId = id,
                            Rating = r.Rating
                        });
                    }
                    else
                    {
                        var registration = _context.Registrations.Find(r.RegistrationId);
                        registration.Rating = r.Rating;
                    }
                }
                _context.SaveChanges();
                token.LastValidRegistration = DateTime.UtcNow;
                _context.SaveChanges();
            }
            // Die restlichen Daten für die Anzeige (Statistik) nachladen, denn es werden nur die
            // Formulardaten übermittelt.
            await InitializeAsync(token);
            return Page();
        }

        /// <summary>
        /// Liest die Daten für die Anzeige der Seite aus der Datenbank.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task InitializeAsync(Token token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));

            // Die angemeldeten Fächer des Users auslesen
            var subjectRegistrations = from r in _context.Registrations
                                       where r.TokenId == token.TokenId
                                       select new SubjectRegistration
                                       {
                                           SubjectId = r.SubjectId,
                                           SubjectName = r.Subject.Name,
                                           RegistrationId = r.RegistrationId,
                                           Rating = r.Rating
                                       };
            if (!subjectRegistrations.Any())
            {
                // Wenn keine Anmeldung besteht, wird eine leere Anmeldung für jedes vorhandene
                // Fach erstellt.
                subjectRegistrations = from s in _context.Subjects
                                       select new SubjectRegistration
                                       {
                                           SubjectId = s.SubjectId,
                                           SubjectName = s.Name
                                       };
            }

            RegistrationStats = await (from r in _context.Registrations
                                       where r.Rating == 1
                                       group r by r.Subject.SubjectId into g
                                       orderby g.Key
                                       select new RegistrationStat
                                       {
                                           SubjectId = g.Key,
                                           RegistrationCount = g.Count()
                                       }).ToListAsync();

            ExistingSubjectRegistrations = new SubjectRegistrationList(await subjectRegistrations.ToListAsync());
        }

    }
}
