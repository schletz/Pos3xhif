using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace LibraryManager.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() => typeof(ReservationManager).IsImmutable(), "ReservationManager is immutable.");
                CheckAndWrite(() => typeof(ReservationManager).PropertyHasType<IReadOnlyList<PublicationReservation>>(nameof(ReservationManager.Reservations)), "ReservationManager.Reservations is IReadOnlyList<PublicationReservation>.");

                CheckAndWrite(() => !typeof(PublicationReservation).HasDefaultConstructor(), "PublicationReservation does not have a default constructor.");
                CheckAndWrite(() =>
                    typeof(PublicationReservation).IsReadonlyProperty(nameof(PublicationReservation.Customer))
                    && typeof(PublicationReservation).IsReadonlyProperty(nameof(PublicationReservation.Publication))
                    && typeof(PublicationReservation).IsReadonlyProperty(nameof(PublicationReservation.ReservationFrom)),
                    "PublicationReservation.Customer, Publication and ReservationFrom are read-only.");
                CheckAndWrite(() =>
                    typeof(PublicationReservation).IsReadonlyProperty(nameof(PublicationReservation.MaxReturnDate))
                    && typeof(PublicationReservation).IsReadonlyProperty(nameof(PublicationReservation.IsReturned)),
                    "PublicationReservation.MaxReturnDate and IsReturned are read-only.");
                CheckAndWrite(() => typeof(PublicationReservation).PropertyHasType<DateTime?>(nameof(PublicationReservation.ReturnDate)), "PublicationReservation.ReturnDate is DateTime?.");

                CheckAndWrite(() => !typeof(Customer).HasDefaultConstructor(), "Customer does not have a default constructor.");
                CheckAndWrite(() => typeof(Customer).IsImmutable(), "Customer is immutable.");

                CheckAndWrite(() => !typeof(Publication).HasDefaultConstructor(), "Publication does not have a default constructor.");
                CheckAndWrite(() => typeof(Publication).IsAbstract, "Publication is abstract.");
                CheckAndWrite(() => typeof(Publication).HasProtectedConstructor(typeof(string)), "Publication has a protected constructor.");
                CheckAndWrite(() => typeof(Publication).IsAbstractProperty(nameof(Publication.ReturnAfterDays)), "Publication.ReturnAfterDays is abstract.");
                CheckAndWrite(() => typeof(Publication).IsImmutable(), "Publication is immutable.");

                CheckAndWrite(() => !typeof(Book).HasDefaultConstructor(), "Book does not have a default constructor.");
                CheckAndWrite(() => typeof(Book).IsAssignableTo(typeof(Publication)), "Book extends Publication.");
                CheckAndWrite(() => typeof(Book).IsImmutable(), "Book is immutable.");

                CheckAndWrite(() => !typeof(Magazine).HasDefaultConstructor(), "Magazine does not have a default constructor.");
                CheckAndWrite(() => typeof(Magazine).IsAssignableTo(typeof(Publication)), "Magazine extends Publication.");
                CheckAndWrite(() => typeof(Magazine).IsImmutable(), "Magazine is immutable.");
            }

            {
                Console.WriteLine("Checking constructors");
                var customer = new Customer(cardId: 1, firstname: "Firstname", lastname: "Lastname");
                CheckAndWrite(() => customer.CardId == 1 && customer.Firstname == "Firstname" && customer.Lastname == "Lastname", "Constructor of Customer sets the values.");
                var book = new Book(publisher: "Publisher1", title: "Book", year: 2000);
                CheckAndWrite(() => book.Publisher == "Publisher1" && book.Title == "Book" && book.Year == 2000, "Constructor of Book sets the values.");
                var magazine = new Magazine(publisher: "Publisher2", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));
                CheckAndWrite(() => magazine.Publisher == "Publisher2" && magazine.Name == "Magazine" && magazine.PublicationDate == new DateTime(2020, 11, 30),
                    "Constructor of Magazine sets the values.");
                var reservation = new PublicationReservation(customer, book, new DateTime(2000, 1, 1));
                CheckAndWrite(() =>
                    reservation.Customer == customer && reservation.Publication == book
                    && reservation.ReservationFrom == new DateTime(2000, 1, 1) && reservation.ReturnDate is null,
                    "Constructor of PublicationReservation sets the values.");
            }

            {
                Console.WriteLine("Checking Magazine and Book");
                Publication magazine = new Magazine(publisher: "Publisher", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));
                CheckAndWrite(() => magazine.ReturnAfterDays == 7, "Magazine.ReturnAfterDays is 7.", 2);

                Publication book = new Book(publisher: "Publisher", title: "Book", year: 2000);
                CheckAndWrite(() => book.ReturnAfterDays == 14, "Book.ReturnAfterDays is 14.", 2);
            }

            {
                Console.WriteLine("Checking PublicationReservation");
                var customer = new Customer(cardId: 1, firstname: "Firstname", lastname: "Lastname");
                var book = new Book(publisher: "Publisher", title: "Book", year: 2000);
                var magazine = new Magazine(publisher: "Publisher", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));

                var bookReservation = new PublicationReservation(customer, book, new DateTime(2000, 1, 1));
                var magazineReservation = new PublicationReservation(customer, magazine, new DateTime(2000, 1, 1));
                CheckAndWrite(() => bookReservation.MaxReturnDate == new DateTime(2000, 1, 15), "PublicationReservation.MaxReturnDate is correct for books.", 2);
                CheckAndWrite(() => magazineReservation.MaxReturnDate == new DateTime(2000, 1, 8), "PublicationReservation.MaxReturnDate is correct for magazines.", 2);
                CheckAndWrite(() => !bookReservation.IsReturned, "PublicationReservation.IsReturned is false if ReturnDate is null.", 2);
                CheckAndWrite(() => !bookReservation.IsPendingReservation(new DateTime(2000, 1, 14)), "PublicationReservation.IsPendingReservation returns false if the date is not too late.", 2);
                CheckAndWrite(() => !bookReservation.IsPendingReservation(new DateTime(2000, 1, 15)), "PublicationReservation.IsPendingReservation returns false on MaxReturnDate.");
                CheckAndWrite(() =>
                    bookReservation.IsPendingReservation(new DateTime(2000, 1, 16))
                    && magazineReservation.IsPendingReservation(new DateTime(2000, 1, 9)), "PublicationReservation.IsPendingReservation returns true if the date is too late.", 2);

                bookReservation.ReturnDate = new DateTime(2000, 1, 9);
                CheckAndWrite(() => bookReservation.IsReturned && bookReservation.ReturnDate == new DateTime(2000, 1, 9), "PublicationReservation.IsReturned is true if ReturnDate is set.", 2);
                CheckAndWrite(() => bookReservation.MaxReturnDate == new DateTime(2000, 1, 15), "PublicationReservation.MaxReturnDate does not change when ReturnDate is set.");
                CheckAndWrite(() => !bookReservation.IsPendingReservation(new DateTime(2000, 1, 14)), "PublicationReservation.IsPendingReservation returns false if ReturnDate is set (1).", 2);
                CheckAndWrite(() => !bookReservation.IsPendingReservation(new DateTime(2000, 1, 16)), "PublicationReservation.IsPendingReservation returns false if ReturnDate is set (2).", 2);
                magazineReservation.ReturnDate = new DateTime(2000, 1, 20);
                CheckAndWrite(() => !magazineReservation.IsPendingReservation(new DateTime(2000, 1, 25)), "PublicationReservation.IsPendingReservation returns false if the publication was returned late.");
            }

            {
                Console.WriteLine("Checking ReservationManager");
                var customer1 = new Customer(cardId: 1, firstname: "Firstname", lastname: "Lastname");
                var customer2 = new Customer(cardId: 2, firstname: "Firstname", lastname: "Lastname");

                var book = new Book(publisher: "Publisher", title: "Book", year: 2000);
                var magazine = new Magazine(publisher: "Publisher", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));

                var reservation1 = new PublicationReservation(customer1, book, new DateTime(2000, 1, 1));
                var reservation2 = new PublicationReservation(customer1, magazine, new DateTime(2000, 2, 1));
                var reservation3 = new PublicationReservation(customer2, book, new DateTime(2000, 3, 1));

                var reservationManager = new ReservationManager();
                CheckAndWrite(() => reservationManager.Reservations.Count == 0, "A new ReservationManager has no reservations.");
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation1) && reservationManager.Reservations.Count == 1,
                    "ReservationManager.TryAddReservation adds the first reservation of customer 1.", 2);
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation3) && reservationManager.Reservations.Count == 2,
                    "ReservationManager.TryAddReservation adds a reservation of customer 2 while customer 1 has a reservation.", 2);
                CheckAndWrite(
                    () => !reservationManager.TryAddReservation(reservation2) && reservationManager.Reservations.Count == 2,
                    "ReservationManager.TryAddReservation rejects a reservation of customer 1 if a publication is not returned.", 2);
                CheckAndWrite(
                    () => !reservationManager.TryAddReservation(new PublicationReservation(customer1, magazine, new DateTime(2000, 1, 10)))
                    && reservationManager.Reservations.Count == 2,
                    "ReservationManager.TryAddReservation rejects a reservation of customer 1 before the publication is overdue.", 2);

                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 10)).Count == 0,
                    "ReservationManager.GetPendingReservations returns 0 pending reservations on 2000-01-10.", 2);
                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 15)).Count == 0,
                    "ReservationManager.GetPendingReservations returns 0 pending reservations on MaxReturnDate (2000-01-15).");
                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 20)).Count == 1
                    && reservationManager.GetPendingReservations(new DateTime(2000, 1, 20))[0] == reservation1,
                    "ReservationManager.GetPendingReservations returns 1 pending reservation on 2000-01-20.", 2);
                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 3, 20)).Count == 2,
                    "ReservationManager.GetPendingReservations returns 2 pending reservations on 2000-03-20.", 2);

                reservation1.ReturnDate = new DateTime(2000, 1, 20);
                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 20)).Count == 0
                    && reservationManager.GetPendingReservations(new DateTime(2000, 3, 20)).Count == 1,
                    "ReservationManager.GetPendingReservations ignores returned publications.", 2);
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation2) && reservationManager.Reservations.Count == 3,
                    "ReservationManager.TryAddReservation adds a reservation of customer 1 after the publication is returned.", 2);
                CheckAndWrite(() =>
                    reservationManager.Reservations[0] == reservation1
                    && reservationManager.Reservations[1] == reservation3
                    && reservationManager.Reservations[2] == reservation2, "ReservationManager.Reservations contains the reservations in the order they were added.");
                CheckAndWrite(
                    () => !reservationManager.TryAddReservation(new PublicationReservation(customer1, book, new DateTime(2000, 2, 5)))
                    && reservationManager.Reservations.Count == 3,
                    "ReservationManager.TryAddReservation rejects customer 1 if a later publication is not returned.", 2);

                var customerA = new Customer(cardId: 10, firstname: "Anna", lastname: "Adams");
                var customerB = new Customer(cardId: 10, firstname: "Bert", lastname: "Brown");
                var otherManager = new ReservationManager();
                otherManager.TryAddReservation(new PublicationReservation(customerA, book, new DateTime(2000, 1, 1)));
                CheckAndWrite(
                    () => otherManager.TryAddReservation(new PublicationReservation(customerB, magazine, new DateTime(2000, 1, 1))),
                    "ReservationManager.TryAddReservation compares the Customer object, not the CardId.");
            }

            WriteSummary();
        }
    }
}
