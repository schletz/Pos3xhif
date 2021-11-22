using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace CarRentalService.Application
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() => typeof(Rental).HasDefaultConstructor() == false, "Rental does not have a default constructor.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.Customer)), "Rental.Customer is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.Vehicle)), "Rental.Vehicle is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.KilometersBegin)), "Rental.KilometersBegin is read-only.");

                CheckAndWrite(() => typeof(Customer).HasDefaultConstructor() == false, "Customer does not have a default constructor.");
                CheckAndWrite(() => typeof(Customer).IsImmutable(), "Customer is immutable.");

                CheckAndWrite(() => typeof(Vehicle).HasDefaultConstructor() == false, "Vehicle does not have a default constructor.");
                CheckAndWrite(() => typeof(Vehicle).IsAbstract, "Vehicle is abstract.");
                CheckAndWrite(() => typeof(Vehicle).IsAbstractProperty(nameof(Vehicle.RequiredDrivingLicence)), "Vehicle.RequiredDrivingLicence is abstract.");
                CheckAndWrite(() => typeof(Vehicle).GetMethod(nameof(Vehicle.CalculatePrice))?.IsAbstract == true, "Vehicle.CalculatePrice() is abstract.");

                CheckAndWrite(() => typeof(Motorcycle).HasDefaultConstructor() == false, "Motorcycle does not have a default constructor.");
                CheckAndWrite(() => typeof(Motorcycle).IsImmutable(), "Motorcycle is immutable.");
                CheckAndWrite(() => typeof(Car).HasDefaultConstructor() == false, "Car does not have a default constructor.");
                CheckAndWrite(() => typeof(Car).IsImmutable(), "Car is immutable.");
            }

            {
                Console.WriteLine("Checking Motorcycle implementation");
                var mc = new Motorcycle(licencePlate: "W", pricePerDay: 10, power: 100);
                CheckAndWrite(() => mc.RequiredDrivingLicence == "A", "Required driving licence for motorcycles is A");
                CheckAndWrite(() => mc.CalculatePrice(10, 10) == 100, "Motorcycle.CalculatePrice() returns the correct result.");
            }

            {
                Console.WriteLine("Checking Car implementation");
                var pkw = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 3499);
                CheckAndWrite(() => pkw.RequiredDrivingLicence == "B", "Required driving licence for cars < 3500kg is B");
                CheckAndWrite(() => pkw.CalculatePrice(10, 10) == 150, "Motorcycle.CalculatePrice() returns the correct result.");
                var van = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 3500);
                CheckAndWrite(() => pkw.RequiredDrivingLicence == "B", "Required driving licence for cars >= 3500kg is C");
            }

            {
                Console.WriteLine("Checking Rental implementation");
                var customer = new Customer(firstname: "FN", lastname: "LN");
                var car = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 1500);
                var start = new DateTime(2021, 11, 10);
                var end = new DateTime(2021, 11, 15);
                var rental = new Rental(start: start, customer: customer, vehicle: (Vehicle)car, kilometersBegin: 100);
                CheckAndWrite(() => rental.KilometersEnd is null, "Default value of Rental.KilometersEnd is null.");
                CheckAndWrite(() => rental.End is null, "Default value of Rental.KilometersEnd is null.");
                CheckAndWrite(() => rental.RentedDays is null, "Without end date Rental.RentedDays is null.");
                CheckAndWrite(() => rental.CalculatePrice() == 0, "Without end date and KilometersEnd Rental.CalculatePrice() is 0.");
                rental.End = end;
                CheckAndWrite(() => rental.RentedDays == 6, "Rental.RentedDays returns the correct result.");
                CheckAndWrite(() => rental.CalculatePrice() == 0, "Without KilometersEnd Rental.CalculatePrice() is 0.");
                rental.KilometersEnd = 200;
                CheckAndWrite(() => rental.CalculatePrice() == 6 * 10 + 100 * 5, "Rental.CalculatePrice() returns the correct result.");
            }
            WriteSummary();
        }
    }
}