using System;
using System.Collections.Generic;
using System.Linq;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace TeamCalendar.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            {
                Console.WriteLine("Teste Klassenimplementierung.");
                CheckAndWrite(() => !typeof(User).HasDefaultConstructor(), "Kein Defaultkonstruktor in User.");
                CheckAndWrite(() => !typeof(CalendarItem).HasDefaultConstructor(), "Kein Defaultkonstruktor in CalendarItem.");
                CheckAndWrite(() => !typeof(PrivateItem).HasDefaultConstructor(), "Kein Defaultkonstruktor in PrivateItem.");
                CheckAndWrite(() => !typeof(Meeting).HasDefaultConstructor(), "Kein Defaultkonstruktor in Meeting.");
                CheckAndWrite(() => typeof(CalendarItem).IsAbstract, "CalendarItem ist eine abstrakte Klasse.");
                CheckAndWrite(
                    () => typeof(User).PropertyHasType<IReadOnlyList<CalendarItem>>(nameof(User.CalendarItems)),
                    "User.CalendarItems ist vom Typ IReadOnlyList<CalendarItem>.");
                CheckAndWrite(
                    () => typeof(Meeting).PropertyHasType<IReadOnlyList<User>>(nameof(Meeting.Participants)),
                    "Meeting.Participants ist vom Typ IReadOnlyList<User>.");
            }
            {
                Console.WriteLine("Teste User.TryAddItem");
                var owner = new User(username: "user1");
                CalendarItem item1 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                CheckAndWrite(() => owner.TryAddItem(item1) == true && owner.CalendarItems.Count == 1, "User.TryAddItem fügt den Termin zu CalendarItems hinzu.", 2);

                int oldCount = owner.CalendarItems.Count;
                CalendarItem item2 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 7, 0, 0), end: new DateTime(2021, 1, 1, 8, 30, 0));
                CheckAndWrite(() => oldCount == 1 && item2.IsCollision(item2), "CalendarItem.IsCollision erkennt Kollisionen.");
                CheckAndWrite(() => oldCount == 1 && !owner.TryAddItem(item2) && owner.CalendarItems.Count == 1, "User.TryAddItem lehnt Kollisionen ab.", 2);

                var user = new User(username: "user2");
                CalendarItem item3 = new PrivateItem(owner: user, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                CheckAndWrite(() => !owner.TryAddItem(item3) && owner.CalendarItems.Count == 1, "User.TryAddItem lehnt private Termine von fremden Usern ab.", 2);
            }
            {
                Console.WriteLine("Teste User.RemoveItem");
                var owner = new User(username: "user1");
                PrivateItem item = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(item);
                int oldCount = owner.CalendarItems.Count;
                owner.RemoveItem(item);
                CheckAndWrite(() => oldCount == 1 && owner.CalendarItems.Count == 0, "User.RemoveItem löscht private Termine.", 2);
            }

            {
                Console.WriteLine("Teste Meeting.Invite");
                var owner = new User(username: "user1");
                var participant = new User(username: "user2");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                meeting.Description = "Description";
                owner.TryAddItem(meeting);
                CheckAndWrite(() => meeting.TryInvite(participant) && meeting.Participants.Count == 1, "Meeting.Invite fügt den User zum Meeting hinzu.", 2);
            }
            {
                Console.WriteLine("Teste User.CancelMeeting (participant)");
                var owner = new User(username: "user1");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(meeting);
                var participant = new User(username: "user2");
                meeting.TryInvite(participant);
                var participant2 = new User(username: "user3");
                meeting.TryInvite(participant2);

                int oldParticipantsCount = meeting.Participants.Count;
                int oldItemsCount = participant.CalendarItems.Count;
                participant.CancelMeeting(meeting);
                CheckAndWrite(() => oldParticipantsCount == 2 & meeting.Participants.Count == 1, "User.CancelMeeting löscht einen Teilnehmer aus dem Meeting.");
                CheckAndWrite(() => oldItemsCount == 1 & participant.CalendarItems.Count == 0, "User.CancelMeeting löscht den Termin aus dem Kalender des Teilnehmers.");
                CheckAndWrite(() => owner.CalendarItems.Count == 1, "User.CancelMeeting belässt das Meeting im Kalender des Organisators.");
                CheckAndWrite(() => participant2.CalendarItems.Count == 1, "User.CancelMeeting belässt das Meeting im Kalender des anderen Teilnehmers.");
            }
            {
                Console.WriteLine("Teste User.CancelMeeting (owner)");
                var owner = new User(username: "user1");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(meeting);
                var participant = new User(username: "user2");
                meeting.TryInvite(participant);
                int oldOwnerItemsCount = owner.CalendarItems.Count;
                int oldParticipantsItemsCount = participant.CalendarItems.Count;
                owner.CancelMeeting(meeting);
                CheckAndWrite(() => oldOwnerItemsCount == 1 && owner.CalendarItems.Count == 0, "User.CancelMeeting löscht das Meeting aus dem Kalender des Eigentümers.");
                CheckAndWrite(() => oldParticipantsItemsCount == 1 && participant.CalendarItems.Count == 0, "User.CancelMeeting löscht das Meeting aus dem Kalender des Teilnehmers.");
            }

            WriteSummary();
        }
    }
}
