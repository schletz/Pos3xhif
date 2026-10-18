using System;
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
                CheckAndWrite(() => !typeof(Rental).HasDefaultConstructor(), "Rental does not have a default constructor.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.Start)), "Rental.Start is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.Customer)), "Rental.Customer is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.Vehicle)), "Rental.Vehicle is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.KilometersBegin)), "Rental.KilometersBegin is read-only.");
                CheckAndWrite(() => typeof(Rental).IsReadonlyProperty(nameof(Rental.RentedDays)), "Rental.RentedDays is read-only.");
                CheckAndWrite(() =>
                    typeof(Rental).PropertyHasType<DateTime?>(nameof(Rental.End))
                    && typeof(Rental).PropertyHasType<int?>(nameof(Rental.KilometersEnd))
                    && typeof(Rental).PropertyHasType<int?>(nameof(Rental.RentedDays)), "Rental.End, KilometersEnd and RentedDays are nullable.");

                CheckAndWrite(() => !typeof(Customer).HasDefaultConstructor(), "Customer does not have a default constructor.");
                CheckAndWrite(() => typeof(Customer).IsImmutable(), "Customer is immutable.");

                CheckAndWrite(() => !typeof(Vehicle).HasDefaultConstructor(), "Vehicle does not have a default constructor.");
                CheckAndWrite(() => typeof(Vehicle).IsAbstract, "Vehicle is abstract.");
                CheckAndWrite(() => typeof(Vehicle).HasProtectedConstructor(typeof(string), typeof(decimal), typeof(int)), "Vehicle has a protected constructor.");
                CheckAndWrite(() => typeof(Vehicle).IsImmutable(), "Vehicle is immutable.");
                CheckAndWrite(() =>
                    typeof(Vehicle).PropertyHasType<decimal>(nameof(Vehicle.PricePerDay))
                    && typeof(Vehicle).PropertyHasType<int>(nameof(Vehicle.Power)), "Vehicle.PricePerDay is decimal and Vehicle.Power is int.");
                CheckAndWrite(() => typeof(Vehicle).IsAbstractProperty(nameof(Vehicle.RequiredDrivingLicence)), "Vehicle.RequiredDrivingLicence is abstract.");
                CheckAndWrite(() => typeof(Vehicle).GetMethod(nameof(Vehicle.CalculatePrice))?.IsAbstract == true, "Vehicle.CalculatePrice() is abstract.");

                CheckAndWrite(() => !typeof(Motorcycle).HasDefaultConstructor(), "Motorcycle does not have a default constructor.");
                CheckAndWrite(() => typeof(Motorcycle).IsImmutable(), "Motorcycle is immutable.");
                CheckAndWrite(() => !typeof(Car).HasDefaultConstructor(), "Car does not have a default constructor.");
                CheckAndWrite(() => typeof(Car).IsImmutable(), "Car is immutable.");
                CheckAndWrite(() =>
                    typeof(Car).PropertyHasType<decimal>(nameof(Car.PricePerKm))
                    && typeof(Car).PropertyHasType<int>(nameof(Car.Weight)), "Car.PricePerKm is decimal and Car.Weight is int.");
            }

            {
                Console.WriteLine("Checking constructors");
                var customer = new Customer(firstname: "FN", lastname: "LN");
                CheckAndWrite(() => customer.Firstname == "FN" && customer.Lastname == "LN", "Constructor of Customer sets the values.");
                var motorcycle = new Motorcycle(licencePlate: "W-1", pricePerDay: 10, power: 100);
                CheckAndWrite(() => motorcycle.LicencePlate == "W-1" && motorcycle.PricePerDay == 10 && motorcycle.Power == 100,
                    "Constructor of Motorcycle sets the values.");
                var car = new Car(licencePlate: "W-2", pricePerDay: 20, power: 90, pricePerKm: 0.5M, weight: 1500);
                CheckAndWrite(() =>
                    car.LicencePlate == "W-2" && car.PricePerDay == 20 && car.Power == 90
                    && car.PricePerKm == 0.5M && car.Weight == 1500, "Constructor of Car sets the values.");
                var start = new DateTime(2021, 11, 10);
                var rental = new Rental(start: start, customer: customer, vehicle: car, kilometersBegin: 100);
                CheckAndWrite(() => rental.Start == start && rental.Customer == customer && rental.Vehicle == car && rental.KilometersBegin == 100,
                    "Constructor of Rental sets the values.");
            }

            {
                Console.WriteLine("Checking Motorcycle");
                var motorcycle = new Motorcycle(licencePlate: "W", pricePerDay: 10, power: 100);
                CheckAndWrite(() => motorcycle.RequiredDrivingLicence == "A", "The required driving licence for motorcycles is A.");
                CheckAndWrite(() => motorcycle.CalculatePrice(3, 50) == 30, "Motorcycle.CalculatePrice() returns days x PricePerDay.", 2);
            }

            {
                Console.WriteLine("Checking Car");
                var car = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 3499);
                CheckAndWrite(() => car.RequiredDrivingLicence == "B", "The required driving licence for cars < 3500 kg is B.");
                CheckAndWrite(() => car.CalculatePrice(3, 50) == 280, "Car.CalculatePrice() returns days x PricePerDay + km x PricePerKm.", 2);
                var truck = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 3500);
                CheckAndWrite(() => truck.RequiredDrivingLicence == "C", "The required driving licence for cars >= 3500 kg is C.");
            }

            {
                Console.WriteLine("Checking Rental");
                var customer = new Customer(firstname: "FN", lastname: "LN");
                var car = new Car(licencePlate: "W", pricePerDay: 10, power: 100, pricePerKm: 5, weight: 1500);
                var rental = new Rental(start: new DateTime(2021, 11, 10), customer: customer, vehicle: car, kilometersBegin: 100);
                CheckAndWrite(() => rental.KilometersEnd is null, "The default value of Rental.KilometersEnd is null.");
                CheckAndWrite(() => rental.End is null, "The default value of Rental.End is null.");
                CheckAndWrite(() => rental.RentedDays is null, "Without an end date Rental.RentedDays is null.");
                CheckAndWrite(() => rental.CalculatePrice() == 0, "Without End and KilometersEnd Rental.CalculatePrice() returns 0.");
                rental.End = new DateTime(2021, 11, 15);
                CheckAndWrite(() => rental.RentedDays == 6, "Rental.RentedDays returns the number of days started.", 2);
                CheckAndWrite(() => rental.CalculatePrice() == 0, "Without KilometersEnd Rental.CalculatePrice() returns 0.");
                rental.KilometersEnd = 200;
                CheckAndWrite(() => rental.CalculatePrice() == 6 * 10 + 100 * 5, "Rental.CalculatePrice() uses the driven kilometers.", 2);

                var rentalNoEnd = new Rental(start: new DateTime(2021, 11, 10), customer: customer, vehicle: car, kilometersBegin: 100);
                rentalNoEnd.KilometersEnd = 200;
                CheckAndWrite(() => rentalNoEnd.CalculatePrice() == 0, "Without End Rental.CalculatePrice() returns 0.");

                var sameDayRental = new Rental(start: new DateTime(2021, 11, 10), customer: customer, vehicle: car, kilometersBegin: 0);
                sameDayRental.End = new DateTime(2021, 11, 10);
                CheckAndWrite(() => sameDayRental.RentedDays == 1, "Rental.RentedDays is 1 if the vehicle is returned on the same day.");

                var monthRental = new Rental(start: new DateTime(2021, 11, 29), customer: customer, vehicle: car, kilometersBegin: 0);
                monthRental.End = new DateTime(2021, 12, 2);
                CheckAndWrite(() => monthRental.RentedDays == 4, "Rental.RentedDays works across the end of a month.");

                var yearRental = new Rental(start: new DateTime(2021, 12, 30), customer: customer, vehicle: car, kilometersBegin: 0);
                yearRental.End = new DateTime(2022, 1, 2);
                CheckAndWrite(() => yearRental.RentedDays == 4, "Rental.RentedDays works across the end of a year.");

                var noKilometersRental = new Rental(start: new DateTime(2021, 11, 10), customer: customer, vehicle: car, kilometersBegin: 0);
                noKilometersRental.End = new DateTime(2021, 11, 10);
                noKilometersRental.KilometersEnd = 0;
                CheckAndWrite(() => noKilometersRental.CalculatePrice() == 10, "Rental.CalculatePrice() charges the days if no kilometers were driven.");

                var motorcycle = new Motorcycle(licencePlate: "W", pricePerDay: 10, power: 100);
                var motorcycleRental = new Rental(start: new DateTime(2021, 11, 10), customer: customer, vehicle: motorcycle, kilometersBegin: 1000);
                motorcycleRental.End = new DateTime(2021, 11, 12);
                motorcycleRental.KilometersEnd = 1300;
                CheckAndWrite(() => motorcycleRental.CalculatePrice() == 30, "Rental.CalculatePrice() uses the price calculation of the rented vehicle.", 2);
            }
            WriteSummary();
        }
    }
}
