using System;
using System.Collections.Generic;
using System.Linq;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace RegistrationSystem.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Checking class implementation");
            CheckAndWrite(() => !typeof(Applicant).HasDefaultConstructor(), "Applicant does not have a default constructor.");
            CheckAndWrite(() => typeof(Applicant).HasProtectedConstructor(typeof(Applicant)), "The copy constructor of Applicant is protected.");
            CheckAndWrite(() => !typeof(AcceptedApplicant).HasDefaultConstructor(), "AcceptedApplicant does not have a default constructor.");
            CheckAndWrite(() => typeof(AcceptedApplicant).IsAssignableTo(typeof(Applicant)), "AcceptedApplicant extends Applicant.");
            CheckAndWrite(() => typeof(RegistrationService).IsImmutable(), "All properties of RegistrationService are read-only.");
            CheckAndWrite(() => typeof(Applicant).IsImmutable(), "All properties of Applicant are read-only.");
            CheckAndWrite(() => typeof(AcceptedApplicant).IsImmutable(), "All properties of AcceptedApplicant are read-only.");
            CheckAndWrite(() => typeof(IGradeChecker).IsInterface, "IGradeChecker is an interface.");
            CheckAndWrite(() => typeof(HtlGradeChecker).IsAssignableTo(typeof(IGradeChecker)), "HtlGradeChecker implements IGradeChecker.");
            CheckAndWrite(() => typeof(FsGradeChecker).IsAssignableTo(typeof(IGradeChecker)), "FsGradeChecker implements IGradeChecker.");
            CheckAndWrite(
                () => typeof(Applicant).PropertyHasType<IReadOnlyDictionary<string, int>>(nameof(Applicant.Grades)),
                "Applicant.Grades is IReadOnlyDictionary<string, int>.");
            CheckAndWrite(
                () => typeof(RegistrationService).PropertyHasType<IReadOnlyCollection<Applicant>>(nameof(RegistrationService.Applicants)),
                "RegistrationService.Applicants is IReadOnlyCollection<Applicant>.");

            Console.WriteLine("Checking grade checkers");
            var htlGradeChecker = new HtlGradeChecker();
            CheckAndWrite(() =>
                htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 2 }, { "M", 3 } })
                && !htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 4 }, { "E", 1 }, { "M", 1 } })
                && !htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 2 }, { "M", 4 } })
                && !htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 5 }, { "M", 1 } }), "HtlGradeChecker.CanBeAccepted returns the correct result.", 2);
            var fsGradeChecker = new FsGradeChecker();
            CheckAndWrite(() =>
                fsGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 2 }, { "M", 4 } })
                && !fsGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 5 }, { "E", 1 }, { "M", 1 } })
                && !fsGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 2 }, { "M", 5 } }), "FsGradeChecker.CanBeAccepted returns the correct result.", 2);
            CheckAndWrite(() =>
                !htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 1 }, { "M", 1 }, { "R", 4 } })
                && htlGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "GGP", 2 }, { "BIU", 3 } }), "HtlGradeChecker.CanBeAccepted checks the grades of all subjects.");
            CheckAndWrite(() =>
                !fsGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "D", 1 }, { "E", 1 }, { "M", 1 }, { "R", 5 } })
                && fsGradeChecker.CanBeAccepted(new Dictionary<string, int> { { "GGP", 4 }, { "BIU", 3 } }), "FsGradeChecker.CanBeAccepted checks the grades of all subjects.");

            Console.WriteLine("Checking applicants");
            var applicant = new Applicant(firstname: "Firstname", lastname: "Lastname", email: "Email", department: "Department");
            CheckAndWrite(() =>
                applicant.Firstname == "Firstname" && applicant.Lastname == "Lastname"
                && applicant.Email == "Email" && applicant.Department == "Department"
                && applicant.Grades.Count == 0, "Constructor of Applicant sets the values.");
            CheckAndWrite(() =>
                applicant.AddGrade("D", 1) && applicant.AddGrade("E", 2) && applicant.AddGrade("M", 3)
                && applicant.Grades.Count == 3 && applicant.Grades["D"] == 1 && applicant.Grades["M"] == 3, "Applicant.AddGrade adds the grades and returns true.", 2);
            CheckAndWrite(() => !applicant.AddGrade("D", 2) && applicant.Grades.Count == 3 && applicant.Grades["D"] == 1, "Applicant.AddGrade rejects a subject that already has a grade.", 2);

            DateTime beforeAccepted = DateTime.UtcNow;
            var acceptedApplicant = new AcceptedApplicant(applicant);
            DateTime afterAccepted = DateTime.UtcNow;
            CheckAndWrite(() =>
                acceptedApplicant.Firstname == "Firstname" && acceptedApplicant.Lastname == "Lastname"
                && acceptedApplicant.Email == "Email" && acceptedApplicant.Department == "Department"
                && acceptedApplicant.Grades.Count == 3 && acceptedApplicant.Grades["E"] == 2, "Constructor of AcceptedApplicant copies the values.", 2);
            CheckAndWrite(() =>
                acceptedApplicant.DateAccepted.Kind == DateTimeKind.Utc
                && acceptedApplicant.DateAccepted >= beforeAccepted && acceptedApplicant.DateAccepted <= afterAccepted, "Constructor of AcceptedApplicant sets DateAccepted to DateTime.UtcNow.");
            CheckAndWrite(() =>
            {
                // Wait until the clock has moved on, so a second instance must get a later time.
                System.Threading.Thread.Sleep(20);
                var secondAccepted = new AcceptedApplicant(applicant);
                return secondAccepted.DateAccepted > acceptedApplicant.DateAccepted;
            }, "Each AcceptedApplicant gets its own DateAccepted.");
            CheckAndWrite(() =>
            {
                applicant.AddGrade("R", 1);
                return applicant.Grades.Count == 4 && acceptedApplicant.Grades.Count == 3;
            }, "The copy constructor copies the grades into a new dictionary.");

            Console.WriteLine("Checking RegistrationService");
            var applicantHif = new Applicant(firstname: "A", lastname: "B", email: "C", department: "HIF");
            applicantHif.AddGrade("D", 1); applicantHif.AddGrade("E", 2); applicantHif.AddGrade("M", 3);

            var applicantHifTooBad = new Applicant(firstname: "A", lastname: "B", email: "D", department: "HIF");
            applicantHifTooBad.AddGrade("D", 1); applicantHifTooBad.AddGrade("E", 2); applicantHifTooBad.AddGrade("M", 4);

            var applicantDuplicateMail = new Applicant(firstname: "X", lastname: "Y", email: "C", department: "FIT");
            applicantDuplicateMail.AddGrade("D", 1); applicantDuplicateMail.AddGrade("E", 2); applicantDuplicateMail.AddGrade("M", 3);

            var applicantFs = new Applicant(firstname: "A", lastname: "B", email: "E", department: "FIT");
            applicantFs.AddGrade("D", 1); applicantFs.AddGrade("E", 2); applicantFs.AddGrade("M", 4);

            var applicantFsTooBad = new Applicant(firstname: "A", lastname: "B", email: "F", department: "FIT");
            applicantFsTooBad.AddGrade("D", 1); applicantFsTooBad.AddGrade("E", 2); applicantFsTooBad.AddGrade("M", 5);

            var applicantNoGradeCheckDepartment = new Applicant(firstname: "A", lastname: "B", email: "G", department: "HBGM");
            applicantNoGradeCheckDepartment.AddGrade("D", 5); applicantNoGradeCheckDepartment.AddGrade("E", 5); applicantNoGradeCheckDepartment.AddGrade("M", 5);

            var registrationService = new RegistrationService();
            CheckAndWrite(() =>
                registrationService.AddApplicant(applicantHif)
                && registrationService.AddApplicant(applicantHifTooBad)
                && registrationService.AddApplicant(applicantFs)
                && registrationService.AddApplicant(applicantFsTooBad)
                && registrationService.AddApplicant(applicantNoGradeCheckDepartment), "RegistrationService.AddApplicant returns true for a new email.", 2);
            CheckAndWrite(() => registrationService.Applicants.Count == 5, "RegistrationService.Applicants contains all applicants.", 2);
            CheckAndWrite(() =>
                !registrationService.AddApplicant(applicantDuplicateMail)
                && registrationService.Applicants.Count == 5
                && registrationService.Applicants.Single(a => a.Email == "C").Department == "HIF", "RegistrationService.AddApplicant rejects an existing email.", 2);
            registrationService.RegisterGradeChecker("HIF", htlGradeChecker);
            registrationService.RegisterGradeChecker("FIT", fsGradeChecker);

            CheckAndWrite(() => registrationService.AcceptApplicant(applicantHif.Email), "RegistrationService.AcceptApplicant accepts an applicant with sufficient grades.", 2);
            CheckAndWrite(() =>
                registrationService.Applicants.Count == 5
                && registrationService.Applicants.Single(a => a.Email == applicantHif.Email) is AcceptedApplicant, "RegistrationService.AcceptApplicant replaces the applicant with an AcceptedApplicant.", 2);
            CheckAndWrite(() => !registrationService.AcceptApplicant("X"), "RegistrationService.AcceptApplicant returns false if the email does not exist.", 2);
            CheckAndWrite(() => !registrationService.AcceptApplicant(applicantHif.Email), "RegistrationService.AcceptApplicant returns false if the applicant is already accepted.", 2);
            CheckAndWrite(() =>
                !registrationService.AcceptApplicant(applicantHifTooBad.Email)
                && registrationService.Applicants.Single(a => a.Email == applicantHifTooBad.Email) is not AcceptedApplicant, "RegistrationService.AcceptApplicant returns false if the grades are not sufficient for the HTL.", 2);
            CheckAndWrite(() =>
                registrationService.AcceptApplicant(applicantFs.Email)
                && !registrationService.AcceptApplicant(applicantFsTooBad.Email), "RegistrationService.AcceptApplicant uses the grade checker of the department (FS).", 2);
            CheckAndWrite(() =>
                registrationService.AcceptApplicant(applicantNoGradeCheckDepartment.Email),
                "RegistrationService.AcceptApplicant ignores the grades if no grade checker is registered for the department.", 2);
            CheckAndWrite(() =>
                registrationService.CountAccepted("HIF") == 1
                && registrationService.CountAccepted("FIT") == 1
                && registrationService.CountAccepted("HBGM") == 1
                && registrationService.CountAccepted("XYZ") == 0, "RegistrationService.CountAccepted counts the accepted applicants of a department.", 2);

            {
                var service = new RegistrationService();
                var applicantWith4 = new Applicant(firstname: "A", lastname: "B", email: "H", department: "HIF");
                applicantWith4.AddGrade("M", 4);
                service.AddApplicant(applicantWith4);
                service.RegisterGradeChecker("HIF", htlGradeChecker);
                service.RegisterGradeChecker("HIF", fsGradeChecker);
                CheckAndWrite(() => service.AcceptApplicant("H"), "RegistrationService.RegisterGradeChecker replaces an existing grade checker.");
            }

            WriteSummary();
        }
    }
}
