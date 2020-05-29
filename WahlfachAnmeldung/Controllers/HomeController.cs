using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using WahlfachAnmeldung.Models;
using WahlfachAnmeldung.ViewModel;

namespace WahlfachAnmeldung.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegistrationContext _context;

        public HomeController(ILogger<HomeController> logger, RegistrationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Registration(string id)
        {
            var token = _context.Tokens.Find(id);
            if (token == null) { return NotFound(); }

            var vm = new RegistrationViewModel
            {
                Token = id,
                LastValidRegistration = token.LastValidRegistration,
                RegistrationStats = _context.GetRegistrationStats(),
                SubjectRegistrations = _context.GetSubjectRegistrations(id)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Registration(RegistrationViewModel vm)
        {
            var token = _context.Tokens.Find(vm.Token);
            if (token == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                foreach (var r in vm.SubjectRegistrations)
                {
                    if (r.RegistrationId == 0)
                    {
                        _context.Registrations.Add(new Registration
                        {
                            Subject = _context.Subjects.Find(r.SubjectId),
                            Token = token,
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
                token.LastValidRegistration = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction("Registration", "Home");
            }
            else
            {
                vm.RegistrationStats = _context.GetRegistrationStats();
                return View(vm);
            }
        }

        public IActionResult List(string id)
        {
            if (_context.TeacherTokens.Find(id) == null) { return NotFound(); }

            var vm = new ListViewModel
            {
                Registrations = _context.GetRegistrationList(),
                RegistrationStats = _context.GetRegistrationStats()
            };

            return View(vm);
        }

        public IActionResult Tsv(string id)
        {
            if (_context.TeacherTokens.Find(id) == null) { return NotFound(); }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Content(ExportToCsv(_context.GetRegistrationList()), "text/plain", Encoding.GetEncoding(1252));
        }

        private string ExportToCsv<T>(IEnumerable<T> data)
        {
            List<string> lines = new List<string>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            lines.Add(string.Join('\t', properties.Select(p => p.Name.ToUpper())));
            foreach (T row in data)
            {
                lines.Add(string.Join('\t', properties.Select(p => p.GetValue(row)?.ToString() ?? "")));
            }
            return string.Join(Environment.NewLine, lines);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
