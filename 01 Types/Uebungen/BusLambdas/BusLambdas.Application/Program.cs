using System;
using System.Collections.Generic;
using static TestHelpers.ProgramChecker;

namespace BusLambdas.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Every check creates its own list, so a wrong implementation cannot change the data
            // of the following checks.
            {
                Console.WriteLine("Checking part A: TripListFunctions");
                CheckAndWrite(() =>
                {
                    var trips = TripData.Create();
                    var a2Trips = TripListFunctions.Filter(trips, t => t.LineNumber == "A2");
                    var none = TripListFunctions.Filter(trips, t => t.Id > 100);
                    return Ids(a2Trips) == "3,5" && none.Count == 0 && trips.Count == 7;
                }, "Filter returns a new list with the matching trips in the same order.");
                CheckAndWrite(() =>
                {
                    var plates = TripListFunctions.Map(TripData.Create(), t => t.LicensePlate);
                    return string.Join(",", plates) == "DO-AUTO1,DO-AUTO1,DO-AUTO2,DO-BUS40,DO-AUTO2,DO-AUTO3,DO-AUTO3";
                }, "Map returns the projected value of every trip in the same order.");
                CheckAndWrite(() =>
                {
                    var trips = TripData.Create();
                    int idSum = 0;
                    var visited = new List<int>();
                    TripListFunctions.ForEach(trips, t => idSum += t.Id);
                    TripListFunctions.ForEach(trips, t => visited.Add(t.Id));
                    return idSum == 28 && string.Join(",", visited) == "1,2,3,4,5,6,7";
                }, "ForEach calls the action for every trip.");
                CheckAndWrite(() =>
                    TripListFunctions.Exists(TripData.Create(), t => t.LineNumber == "A3")
                    && !TripListFunctions.Exists(TripData.Create(), t => t.LineNumber == "N1"),
                    "Exists returns true if at least one trip matches.");
                CheckAndWrite(() =>
                    TripListFunctions.Count(TripData.Create(), t => t.IsCancelled) == 1
                    && TripListFunctions.Count(TripData.Create(), t => t.PassengerCount == null) == 3
                    && TripListFunctions.Count(TripData.Create(), t => t.Id > 100) == 0,
                    "Count returns the number of matching trips.");
                CheckAndWrite(() =>
                    TripListFunctions.FindFirst(TripData.Create(), t => t.LicensePlate == "DO-AUTO3")?.Id == 6
                    && TripListFunctions.FindFirst(TripData.Create(), t => t.LicensePlate == "W-12345") is null,
                    "FindFirst returns the first matching trip or null.");
                CheckAndWrite(() =>
                    TripListFunctions.Sum(TripData.Create(), t => t.Id) == 28
                    && TripListFunctions.Sum(new List<Trip>(), t => t.Id) == 0,
                    "Sum adds up the selected values.");
            }

            {
                Console.WriteLine("Checking part B: TripRules");
                CheckAndWrite(() =>
                    TripRules.IsCancelled(TripWithId(5)) && !TripRules.IsCancelled(TripWithId(6)),
                    "IsCancelled is true only for cancelled trips.");
                CheckAndWrite(() =>
                    TripRules.HasDeparted(TripWithId(1)) && !TripRules.HasDeparted(TripWithId(6)),
                    "HasDeparted is true if ActualDeparture is set.");
                CheckAndWrite(() =>
                    TripRules.IsDelayed(TripWithId(2))         // 6 minutes
                    && TripRules.IsDelayed(TripWithId(4))      // 3 minutes
                    && !TripRules.IsDelayed(TripWithId(7))     // exactly 2 minutes
                    && !TripRules.IsDelayed(TripWithId(3))     // on time
                    && !TripRules.IsDelayed(TripWithId(6)),    // not departed
                    "IsDelayed is true for a delay of more than 2 minutes.", 2);
                CheckAndWrite(() =>
                    TripRules.PassengersOrZero(TripWithId(4)) == 31 && TripRules.PassengersOrZero(TripWithId(7)) == 0,
                    "PassengersOrZero returns 0 if nobody counted.");
                CheckAndWrite(() =>
                    TripRules.Label(TripWithId(1)) == "A1 07:00" && TripRules.Label(TripWithId(7)) == "A3 10:00",
                    "Label returns line number and planned departure (HH:mm).");
                CheckAndWrite(() =>
                {
                    var original = TripWithId(6);
                    var cancelled = TripRules.Cancel(original);
                    return cancelled.IsCancelled && !original.IsCancelled && cancelled == original with { IsCancelled = true };
                }, "Cancel returns a cancelled copy and does not change the original trip.");
                CheckAndWrite(() =>
                    TripRules.SameVehicle(TripWithId(1), TripWithId(2)) && !TripRules.SameVehicle(TripWithId(1), TripWithId(3)),
                    "SameVehicle compares the license plates of two trips.");
                CheckAndWrite(() =>
                {
                    var isOnA1 = TripRules.IsOnLine("A1");
                    var isOnA3 = TripRules.IsOnLine("A3");
                    return isOnA1(TripWithId(1)) && !isOnA1(TripWithId(4)) && isOnA3(TripWithId(4));
                }, "IsOnLine returns a rule for the given line.", 2);
                CheckAndWrite(() =>
                {
                    var log = new List<string>();
                    var logAction = TripRules.LogTo(log);
                    logAction(TripWithId(1));
                    logAction(TripWithId(3));
                    return string.Join(",", log) == "A1 07:00,A2 07:15";
                }, "LogTo returns an action that adds the label to the list.", 2);
            }

            {
                Console.WriteLine("Checking part C: TripReport");
                CheckAndWrite(() =>
                    string.Join(",", TripReport.GetLabelsOfDelayedTrips(TripData.Create())) == "A1 07:30,A3 08:00",
                    "GetLabelsOfDelayedTrips returns the labels of the delayed trips.", 2);
                CheckAndWrite(() =>
                    TripReport.CountPassengersOnLine(TripData.Create(), "A1") == 21
                    && TripReport.CountPassengersOnLine(TripData.Create(), "A3") == 31
                    && TripReport.CountPassengersOnLine(TripData.Create(), "N1") == 0,
                    "CountPassengersOnLine adds up the passengers of a line (not counted = 0).", 2);
                CheckAndWrite(() =>
                    string.Join(",", TripReport.GetLicensePlatesOfWaitingTrips(TripData.Create())) == "DO-AUTO3",
                    "GetLicensePlatesOfWaitingTrips returns the vehicles of trips that are neither cancelled nor departed.", 2);
            }
            WriteSummary();
        }

        /// <summary>
        /// Returns the trip with the given id from a new list of sample trips.
        /// </summary>
        private static Trip TripWithId(int id) => TripData.Create()[id - 1];

        /// <summary>
        /// Returns the ids of the trips as comma separated text, e.g. "3,5".
        /// </summary>
        private static string Ids(List<Trip> trips) => string.Join(",", trips.ConvertAll(t => t.Id));
    }
}
