using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace HealthChecker.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() => !typeof(Employee).HasDefaultConstructor(), "Employee does not have a default constructor.");
                CheckAndWrite(() => typeof(Employee).IsImmutable(), "Employee is immutable.");
                CheckAndWrite(() => typeof(Employee).PropertyHasType<IReadOnlyList<ISafetyCheck>>(nameof(Employee.SafetyChecks)), "Employee.SafetyChecks is IReadOnlyList<ISafetyCheck>.");

                CheckAndWrite(() => typeof(ISafetyCheck).IsInterface, "ISafetyCheck is an interface.");

                CheckAndWrite(() => !typeof(Test).HasDefaultConstructor(), "Test does not have a default constructor.");
                CheckAndWrite(() => typeof(Test).IsImmutable(), "Test is immutable.");
                CheckAndWrite(() => typeof(Test).IsAssignableTo(typeof(ISafetyCheck)), "Test implements ISafetyCheck.");

                CheckAndWrite(() => !typeof(Cured).HasDefaultConstructor(), "Cured does not have a default constructor.");
                CheckAndWrite(() => typeof(Cured).IsImmutable(), "Cured is immutable.");
                CheckAndWrite(() => typeof(Cured).IsAssignableTo(typeof(ISafetyCheck)), "Cured implements ISafetyCheck.");

                CheckAndWrite(() => !typeof(Vaccination).HasDefaultConstructor(), "Vaccination does not have a default constructor.");
                CheckAndWrite(() => typeof(Vaccination).IsAssignableTo(typeof(ISafetyCheck)), "Vaccination implements ISafetyCheck.");
                CheckAndWrite(() => typeof(Vaccination).IsReadonlyProperty(nameof(Vaccination.FirstVaccination)), "Vaccination.FirstVaccination is read-only.");
                CheckAndWrite(() => typeof(Vaccination).PropertyHasType<DateTime?>(nameof(Vaccination.SecondVaccination)), "Vaccination.SecondVaccination is DateTime?.");
            }

            {
                Console.WriteLine("Checking constructors");
                var employee = new Employee(firstname: "Firstname", lastname: "Lastname");
                CheckAndWrite(() => employee.Firstname == "Firstname" && employee.Lastname == "Lastname" && employee.SafetyChecks.Count == 0,
                    "Constructor of Employee sets the values.");
                var test = new Test(testDate: new DateTime(2021, 11, 1, 8, 0, 0));
                CheckAndWrite(() => test.TestDate == new DateTime(2021, 11, 1, 8, 0, 0), "Constructor of Test sets the values.");
                var cured = new Cured(dateOfDiagnosis: new DateTime(2021, 10, 1));
                CheckAndWrite(() => cured.DateOfDiagnosis == new DateTime(2021, 10, 1), "Constructor of Cured sets the values.");
                var vaccination = new Vaccination(firstVaccination: new DateTime(2021, 5, 1));
                CheckAndWrite(() => vaccination.FirstVaccination == new DateTime(2021, 5, 1) && vaccination.SecondVaccination is null,
                    "Constructor of Vaccination sets the values.");
            }

            {
                Console.WriteLine("Checking Vaccination");
                var checkDate = new DateTime(2021, 11, 1);
                var vaccination = new Vaccination(firstVaccination: new DateTime(2021, 5, 1));
                CheckAndWrite(() => !vaccination.IsValid(checkDate), "Only 1 vaccination is not a valid proof.", 2);
                vaccination.SecondVaccination = new DateTime(2021, 6, 1);
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Vaccination.SecondVaccination can be set.");
                CheckAndWrite(() => vaccination.IsValid(checkDate), "Vaccination.IsValid returns true with valid values.", 2);

                vaccination.SecondVaccination = new DateTime(2021, 6, 3);
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Vaccination.SecondVaccination cannot be overwritten.", 2);
                vaccination.SecondVaccination = new DateTime(2021, 5, 15);
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Vaccination.SecondVaccination cannot be overwritten with an earlier date.");
                vaccination.SecondVaccination = null;
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Vaccination.SecondVaccination cannot be reset to null.");

                var oldVaccination = new Vaccination(firstVaccination: checkDate.AddDays(-720));
                oldVaccination.SecondVaccination = checkDate.AddDays(-361);
                CheckAndWrite(() => !oldVaccination.IsValid(checkDate), "The vaccination expires after 360 days.", 2);

                var vaccination360Days = new Vaccination(firstVaccination: checkDate.AddDays(-400));
                vaccination360Days.SecondVaccination = checkDate.AddDays(-360);
                CheckAndWrite(() => vaccination360Days.IsValid(checkDate), "The vaccination is valid exactly 360 days after the second vaccination.");

                var oldFirstVaccination = new Vaccination(firstVaccination: checkDate.AddDays(-400));
                oldFirstVaccination.SecondVaccination = checkDate.AddDays(-100);
                CheckAndWrite(() => oldFirstVaccination.IsValid(checkDate), "Vaccination.IsValid uses the date of the second vaccination.", 2);

                var invalidSecondVaccination = new Vaccination(firstVaccination: new DateTime(2021, 1, 1));
                invalidSecondVaccination.SecondVaccination = new DateTime(2020, 12, 31);
                CheckAndWrite(() => invalidSecondVaccination.SecondVaccination is null, "Vaccination.SecondVaccination must be after FirstVaccination.", 2);
                invalidSecondVaccination.SecondVaccination = new DateTime(2021, 1, 1);
                CheckAndWrite(() => invalidSecondVaccination.SecondVaccination is null, "Vaccination.SecondVaccination cannot be the same date as FirstVaccination.");
                invalidSecondVaccination.SecondVaccination = new DateTime(2021, 2, 1);
                CheckAndWrite(() => invalidSecondVaccination.SecondVaccination == new DateTime(2021, 2, 1), "Vaccination.SecondVaccination can be set after an ignored assignment.");

                var secondVaccinationInFuture = new Vaccination(firstVaccination: checkDate.AddDays(-10));
                secondVaccinationInFuture.SecondVaccination = checkDate.AddDays(10);
                CheckAndWrite(() => !secondVaccinationInFuture.IsValid(checkDate), "Vaccination.IsValid returns false if the second vaccination is in the future.", 2);

                var secondVaccinationAtCheckDate = new Vaccination(firstVaccination: checkDate.AddDays(-30));
                secondVaccinationAtCheckDate.SecondVaccination = checkDate;
                CheckAndWrite(() => secondVaccinationAtCheckDate.IsValid(checkDate), "Vaccination.IsValid returns true at the time of the second vaccination.");
            }

            {
                Console.WriteLine("Checking Cured");
                var checkDate = new DateTime(2021, 11, 1);
                var cured = new Cured(dateOfDiagnosis: checkDate.AddDays(-89));
                CheckAndWrite(() => cured.IsValid(checkDate), "Cured.IsValid returns true with a valid date.", 2);
                var curedExpired = new Cured(dateOfDiagnosis: checkDate.AddDays(-91));
                CheckAndWrite(() => !curedExpired.IsValid(checkDate), "Cured.IsValid returns false with an expired date.", 2);
                var curedInFuture = new Cured(dateOfDiagnosis: checkDate.AddDays(1));
                CheckAndWrite(() => !curedInFuture.IsValid(checkDate), "Cured.IsValid returns false if the date of diagnosis is in the future.", 2);
                var cured90Days = new Cured(dateOfDiagnosis: checkDate.AddDays(-90));
                var curedToday = new Cured(dateOfDiagnosis: checkDate);
                CheckAndWrite(() => cured90Days.IsValid(checkDate) && curedToday.IsValid(checkDate), "Cured.IsValid returns true exactly 90 days and 0 days after the diagnosis.");
                var curedExpiredByOneHour = new Cured(dateOfDiagnosis: checkDate.AddDays(-90).AddHours(-1));
                CheckAndWrite(() => !curedExpiredByOneHour.IsValid(checkDate), "Cured.IsValid returns false 90 days and 1 hour after the diagnosis.");
            }

            {
                Console.WriteLine("Checking Test");
                var checkDate = new DateTime(2021, 11, 1);
                var test = new Test(testDate: checkDate.AddHours(-47));
                CheckAndWrite(() => test.IsValid(checkDate), "Test.IsValid returns true with a valid date.", 2);
                var testExpired = new Test(testDate: checkDate.AddHours(-49));
                CheckAndWrite(() => !testExpired.IsValid(checkDate), "Test.IsValid returns false if the test is expired.", 2);
                var testInFuture = new Test(testDate: checkDate.AddHours(1));
                CheckAndWrite(() => !testInFuture.IsValid(checkDate), "Test.IsValid returns false if the test date is in the future.", 2);
                var test48Hours = new Test(testDate: checkDate.AddHours(-48));
                var testNow = new Test(testDate: checkDate);
                CheckAndWrite(() => test48Hours.IsValid(checkDate) && testNow.IsValid(checkDate), "Test.IsValid returns true exactly 48 hours and 0 hours after the test.");
            }

            {
                Console.WriteLine("Checking Employee");
                var checkDate = new DateTime(2021, 11, 1);

                var employee = new Employee(firstname: "Firstname", lastname: "Lastname");
                CheckAndWrite(() => !employee.IsSafe(checkDate), "Employee.IsSafe returns false if there is no safety check.", 2);

                var expiredTest = new Test(testDate: checkDate.AddDays(-10));
                employee.AddSafetyCheck(expiredTest);
                CheckAndWrite(() => !employee.IsSafe(checkDate), "Employee.IsSafe returns false if there is only an expired safety check.", 2);
                CheckAndWrite(() => employee.SafetyChecks.Count == 1 && employee.SafetyChecks[0] == expiredTest, "Employee.AddSafetyCheck adds the safety check.", 2);

                employee.AddSafetyCheck(new Test(testDate: checkDate.AddHours(-1)));
                CheckAndWrite(() => employee.IsSafe(checkDate) && employee.SafetyChecks.Count == 2, "Employee.IsSafe returns true if there is a valid safety check.", 2);

                var employee2 = new Employee(firstname: "Firstname", lastname: "Lastname");
                employee2.AddSafetyCheck(new Cured(dateOfDiagnosis: checkDate.AddDays(-10)));
                employee2.AddSafetyCheck(new Test(testDate: checkDate.AddDays(-10)));
                CheckAndWrite(() => employee2.IsSafe(checkDate), "Employee.IsSafe returns true if any safety check is valid (not only the last one).", 2);
                CheckAndWrite(() => !employee2.IsSafe(checkDate.AddDays(100)), "Employee.IsSafe uses the passed date.");
            }

            WriteSummary();
        }
    }
}
