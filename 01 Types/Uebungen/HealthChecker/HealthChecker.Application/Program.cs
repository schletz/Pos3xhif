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
                CheckAndWrite(() => typeof(Employee).HasDefaultConstructor() == false, "Employee does not have a default constructor.");
                CheckAndWrite(() => typeof(Employee).IsImmutable(), "Employee is immutable.");
                CheckAndWrite(() => typeof(Employee).PropertyHasType<IReadOnlyList<ISafetyCheck>>(nameof(Employee.SafetyChecks)), "Employee.SafetyChecks is IReadOnlyList<ISafetyCheck>.");

                CheckAndWrite(() => typeof(ISafetyCheck).IsInterface, "ISafetyCheck is an interface.");

                CheckAndWrite(() => typeof(Test).HasDefaultConstructor() == false, "Test does not have a default constructor.");
                CheckAndWrite(() => typeof(Test).IsImmutable(), "Test is immutable.");
                CheckAndWrite(() => typeof(ISafetyCheck).IsAssignableFrom(typeof(Test)), "Test implements ISafetyCheck.");

                CheckAndWrite(() => typeof(Cured).HasDefaultConstructor() == false, "Cured does not have a default constructor.");
                CheckAndWrite(() => typeof(Cured).IsImmutable(), "Cured is immutable.");
                CheckAndWrite(() => typeof(ISafetyCheck).IsAssignableFrom(typeof(Cured)), "Cured implements ISafetyCheck.");

                CheckAndWrite(() => typeof(Vaccination).HasDefaultConstructor() == false, "Vaccination does not have a default constructor.");
                CheckAndWrite(() => typeof(ISafetyCheck).IsAssignableFrom(typeof(Vaccination)), "Vaccination implements ISafetyCheck.");
                CheckAndWrite(() => typeof(Vaccination).GetProperty(nameof(Vaccination.FirstVaccination))?.CanWrite == false, "FirstVaccination is read-only.");
            }

            {
                Console.WriteLine("Checking vaccination");
                var checkDate = new DateTime(2021, 11, 1);
                var vaccination = new Vaccination(firstVaccination: new DateTime(2021, 5, 1));
                CheckAndWrite(() => !vaccination.IsValid(checkDate), "Only 1 vaccination is not a valid proof.");
                vaccination.SecondVaccination = new DateTime(2021, 6, 1);
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Date of the 2nd vaccination.");
                CheckAndWrite(() => vaccination.IsValid(checkDate), "IsValid works well with valid values.");

                vaccination.SecondVaccination = new DateTime(2021, 6, 3);
                CheckAndWrite(() => vaccination.SecondVaccination == new DateTime(2021, 6, 1), "Date of the second vaccination cannot be overwritten.");

                var oldVaccination = new Vaccination(firstVaccination: checkDate.AddDays(-720));
                oldVaccination.SecondVaccination = checkDate.AddDays(-361);
                CheckAndWrite(() => !oldVaccination.IsValid(checkDate), "The vaccination expires after 360 days.");

                var invalidSecondVaccination = new Vaccination(firstVaccination: new DateTime(2021, 1, 1));
                invalidSecondVaccination.SecondVaccination = new DateTime(2020, 12, 31);
                CheckAndWrite(() => invalidSecondVaccination.SecondVaccination is null, "Property SecondVaccination checks FirstVaccination before assignment.");

                var secondVaccinationnFuture = new Vaccination(firstVaccination: checkDate.AddDays(10));
                invalidSecondVaccination.SecondVaccination = checkDate.AddDays(30);
                CheckAndWrite(() => !invalidSecondVaccination.IsValid(checkDate), "Vaccination is invalid if second vaccination is in future.");
            }

            {
                Console.WriteLine("Checking Cured");
                var checkDate = new DateTime(2021, 11, 1);
                var cured = new Cured(dateOfDiagnosis: checkDate.AddDays(-89));
                CheckAndWrite(() => cured.IsValid(checkDate), "Cured.IsValid returns true with a valid date.");
                var curedExpired = new Cured(dateOfDiagnosis: checkDate.AddDays(-91));
                CheckAndWrite(() => !curedExpired.IsValid(checkDate), "Cured.IsValid returns false with an expired date.");
                var curedInFuture = new Cured(dateOfDiagnosis: checkDate.AddDays(1));
                CheckAndWrite(() => !curedInFuture.IsValid(checkDate), "Cured.IsValid returns false if curedDate is in future.");
            }

            {
                Console.WriteLine("Checking Test");
                var checkDate = new DateTime(2021, 11, 1);
                var test = new Test(testDate: checkDate.AddHours(-47));
                CheckAndWrite(() => test.IsValid(checkDate), "Test.IsValid returns true with a valid date.");
                var testExpired = new Test(testDate: checkDate.AddHours(-49));
                CheckAndWrite(() => !testExpired.IsValid(checkDate), "Cured.IsValid returns false if the test is expired.");
                var testInFuture = new Test(testDate: checkDate.AddHours(1));
                CheckAndWrite(() => !testInFuture.IsValid(checkDate), "Cured.IsValid returns false if test date is in future.");
            }

            {
                Console.WriteLine("Checking Employee");
                var checkDate = new DateTime(2021, 11, 1);

                var employee = new Employee(firstname: "Firstname", lastname: "Lastname");
                CheckAndWrite(() => !employee.IsSafe(checkDate), "Employee.IsSafe returns false is no health check is present.");

                employee.AddSafetyCheck(new Test(testDate: checkDate.AddDays(-10)));
                CheckAndWrite(() => !employee.IsSafe(checkDate), "Employee.IsSafe returns false if an outdated health check is present.");
                CheckAndWrite(() => employee.SafetyChecks.Count == 1, "Safety check is saved.");

                employee.AddSafetyCheck(new Test(testDate: checkDate.AddHours(-1)));
                CheckAndWrite(() => employee.IsSafe(checkDate), "Employee.IsSafe returns true if a valid health check is present.");
            }

            WriteSummary();
        }
    }
}