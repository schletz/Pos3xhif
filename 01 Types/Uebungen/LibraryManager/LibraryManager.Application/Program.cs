using System;
using System.Collections.Generic;
using System.Linq;
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
                CheckAndWrite(() => typeof(ReservationManager).IsImmutable(), "ReservatrionManager is immutable.");
                CheckAndWrite(() => typeof(ReservationManager).PropertyHasType<IReadOnlyList<PublicationReservation>>(nameof(ReservationManager.Reservations)), "ReservatrionManager.Reservations is IReadOnlyList<PublicationReservation>.");

                CheckAndWrite(() => typeof(PublicationReservation).HasDefaultConstructor() == false, "PublicationReservation does not have a default constructor.");

                CheckAndWrite(() => typeof(Customer).HasDefaultConstructor() == false, "Customer does not have a default constructor.");
                CheckAndWrite(() => typeof(Customer).IsImmutable(), "Customer is immutable.");

                CheckAndWrite(() => typeof(Publication).HasDefaultConstructor() == false, "Publication does not have a default constructor.");
                CheckAndWrite(() => typeof(Publication).IsAbstract, "Publication is abstract.");
                CheckAndWrite(() => typeof(Publication).IsImmutable(), "Publication is immutable.");

                CheckAndWrite(() => typeof(Book).HasDefaultConstructor() == false, "Book does not have a default constructor.");
                CheckAndWrite(() => typeof(Publication).IsAssignableFrom(typeof(Book)), "Book extends Publication.");
                CheckAndWrite(() => typeof(Book).IsImmutable(), "Book is immutable.");

                CheckAndWrite(() => typeof(Magazine).HasDefaultConstructor() == false, "Book does not have a default constructor.");
                CheckAndWrite(() => typeof(Publication).IsAssignableFrom(typeof(Magazine)), "Magazine extends Publication.");
                CheckAndWrite(() => typeof(Magazine).IsImmutable(), "Magazine is immutable.");
            }

            {
                Console.WriteLine("Checking class Magazine and Book ");
                var magazine = new Magazine(publisher: "Publisher", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));
                CheckAndWrite(() => ((Publication)magazine).ReturnAfterDays == 7, "Magazine.ReturnAfterDays is 7", 2);

                var book = new Book(publisher: "Publisher", title: "Book", year: 2000);
                CheckAndWrite(() => ((Publication)book).ReturnAfterDays == 14, "Book.ReturnAfterDays is 14", 2);
            }

            {
                Console.WriteLine("Checking PublicationReservation");
                var customer = new Customer(cardId: 1, firstname: "Firstname", lastname: "Lastname");
                var book = new Book(publisher: "Publisher", title: "Book", year: 2000);
                var magazine = new Magazine(publisher: "Publisher", name: "Magazine", publicationDate: new DateTime(2020, 11, 30));

                var bookReservation = new PublicationReservation(customer, (Publication)book, new DateTime(2000, 1, 1));
                var magazineReservation = new PublicationReservation(customer, (Publication)magazine, new DateTime(2000, 1, 1));
                CheckAndWrite(() => bookReservation.ReturnDate is null, "PublicationReservation.ReturnDate is null on a pending reservation.", 1);
                CheckAndWrite(() => bookReservation.MaxReturnDate == new DateTime(2000,1,15), "PublicationReservation.IsReturned is correct for books.", 2);
                CheckAndWrite(() => magazineReservation.MaxReturnDate == new DateTime(2000,1,8), "PublicationReservation.IsReturned is correct for magazines.", 2);
                CheckAndWrite(() => bookReservation.IsReturned == false, "PublicationReservation.IsReturned is false if ReturnDate is null");
                CheckAndWrite(() => bookReservation.IsPendingReservation(new DateTime(2000, 1, 14)) == false, "PublicationReservation.IsPendingReservation returns false if date is not too late.", 2);
                CheckAndWrite(() => bookReservation.IsPendingReservation(new DateTime(2000, 1, 16)) == true, "PublicationReservation.IsPendingReservation returns true if date is too late.", 2);

                bookReservation.ReturnDate = new DateTime(2000, 1, 9);
                CheckAndWrite(() => bookReservation.IsPendingReservation(new DateTime(2000, 1, 14)) == false, "PublicationReservation.IsPendingReservation returns false if ReturnDate is set (1).", 2);
                CheckAndWrite(() => bookReservation.IsPendingReservation(new DateTime(2000, 1, 16)) == false, "PublicationReservation.IsPendingReservation returns false if ReturnDate is set (2).", 2);
            }

            {
                Console.WriteLine("Checking ReservatrionManager");
                var customer1 = new Customer(cardId: 1, firstname: "Firstname", lastname: "Lastname");
                var customer2 = new Customer(cardId: 2, firstname: "Firstname", lastname: "Lastname");

                var book = new Book(publisher: "Publisher", title: "Book", year: 2000);

                var reservation1 = new PublicationReservation(customer1, (Publication)book, new DateTime(2000, 1, 1));
                var reservation2 = new PublicationReservation(customer1, (Publication)book, new DateTime(2000, 2, 1));
                var reservation3 = new PublicationReservation(customer2, (Publication)book, new DateTime(2000, 3, 1));

                var reservationManager = new ReservationManager();
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation1) == true && reservationManager.Reservations.Count == 1, 
                    "ReservatrionManager.TryAddReservation adds first reservation from customer 1.", 2);
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation3) == true && reservationManager.Reservations.Count == 2, 
                    "ReservatrionManager.TryAddReservation adds first reservation from customer 2.", 2);
                CheckAndWrite(
                    () => reservationManager.TryAddReservation(reservation2) == false && reservationManager.Reservations.Count == 2, 
                    "ReservatrionManager.TryAddReservation declines reservation from customer 1 if a pending reservation is present.", 2);

                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 10)).Count == 0, 
                    "ReservatrionManager.GetPendingReservations returns 0 pending reservations on 2000-01-10.", 2);
                CheckAndWrite(
                    () => reservationManager.GetPendingReservations(new DateTime(2000, 1, 20)).Count == 1, 
                    "ReservatrionManager.GetPendingReservations returns 1 pending reservation on 2000-01-20.", 2);
                CheckAndWrite(
                    () => 
                    reservationManager.GetPendingReservations(new DateTime(2000, 1, 20)).Count == 1 
                    && reservationManager.GetPendingReservations(new DateTime(2000, 1, 20))[0].Customer == customer1, 
                    "ReservatrionManager.GetPendingReservations returns the correct customer", 2);

                reservation1.ReturnDate = new DateTime(2020, 1, 20);
                CheckAndWrite(() => reservationManager.GetPendingReservations(new DateTime(2000, 1, 20)).Count == 0, "ReservatrionManager.GetPendingReservations returns 0 pending reservation on 2000-01-20 after returning the book.", 2);


            }


            WriteSummary();
        }
    }
}