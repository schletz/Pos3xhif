using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WahlfachAnmeldung.Models;

namespace WahlfachAnmeldung.Controllers
{
    /// <summary>
    /// Controller für die Exporte als Tab-Getrennte Werte (TSV). Benötigt
    /// endpoints.MapControllers();
    /// in der UseEndpoints Methode in Startup.cs
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TsvController : ControllerBase
    {
        private readonly RegistrationContext _context;

        public TsvController(RegistrationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exportiert die eingetragenen Erstregistierungen.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{token}")]
        public async Task<IActionResult> GetRegistrations(string token)
        {
            // Nur bei gültigem Token werden die Daten angezeigt.
            if (_context.TeacherTokens.Find(token) == null) { return NotFound(); }

            var registrations = await (from r in _context.Registrations
                                 where r.Rating == 1
                                 orderby r.Token.SchoolClass, r.Token.LastValidRegistration
                                 select new
                                 {
                                     Schoolclass = r.Token.SchoolClass,
                                     // Azure hat die englische Locale Einstellung, manuelle Konvertierung auf deutsches Schema.
                                     RegistrationDate = ((DateTime)r.Token.LastValidRegistration).ToString("d.MM.yyyy HH:mm"),
                                     Token = r.Token.TokenId,
                                     Subject = r.Subject.SubjectId
                                 }).ToListAsync();

            // Excel benötigt den Windows 1252 Zeichensatz.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Content(ExportToCsv(registrations), "text/plain", Encoding.GetEncoding(1252));
        }

        /// <summary>
        /// Wandelt eine Liste von Objekten über Reflection in einen String.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ExportToCsv<T>(IEnumerable<T> data)
        {
            char separator = '\t';

            List<string> lines = new List<string>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            lines.Add(string.Join(separator, properties.Select(p => p.Name.ToUpper())));
            foreach (T row in data)
            {
                lines.Add(string.Join(separator, properties.Select(p => p.GetValue(row)?.ToString() ?? "")));
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}