using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace TeamCalendar.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking class implementation");
                CheckAndWrite(() => !typeof(User).HasDefaultConstructor(), "User does not have a default constructor.");
                CheckAndWrite(() => typeof(User).IsImmutable(), "User is immutable.");
                CheckAndWrite(() => !typeof(CalendarItem).HasDefaultConstructor(), "CalendarItem does not have a default constructor.");
                CheckAndWrite(() => !typeof(PrivateItem).HasDefaultConstructor(), "PrivateItem does not have a default constructor.");
                CheckAndWrite(() => !typeof(Meeting).HasDefaultConstructor(), "Meeting does not have a default constructor.");
                CheckAndWrite(() => typeof(CalendarItem).IsAbstract, "CalendarItem is abstract.");
                CheckAndWrite(() => typeof(CalendarItem).HasProtectedConstructor(typeof(User), typeof(string), typeof(DateTime), typeof(DateTime)), "CalendarItem has a protected constructor.");
                CheckAndWrite(() =>
                    typeof(CalendarItem).IsReadonlyProperty(nameof(CalendarItem.Owner))
                    && typeof(CalendarItem).IsReadonlyProperty(nameof(CalendarItem.Title))
                    && typeof(CalendarItem).IsReadonlyProperty(nameof(CalendarItem.Begin))
                    && typeof(CalendarItem).IsReadonlyProperty(nameof(CalendarItem.End)), "CalendarItem.Owner, Title, Begin and End are read-only.");
                CheckAndWrite(
                    () => typeof(User).PropertyHasType<IReadOnlyList<CalendarItem>>(nameof(User.CalendarItems)),
                    "User.CalendarItems is IReadOnlyList<CalendarItem>.");
                CheckAndWrite(
                    () => typeof(Meeting).PropertyHasType<IReadOnlyList<User>>(nameof(Meeting.Participants)),
                    "Meeting.Participants is IReadOnlyList<User>.");
            }
            {
                Console.WriteLine("Checking constructors");
                var owner = new User(username: "user1");
                CheckAndWrite(() => owner.Username == "user1" && owner.CalendarItems.Count == 0, "Constructor of User sets the values.");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                CheckAndWrite(() =>
                    meeting.Owner == owner && meeting.Title == "Title"
                    && meeting.Begin == new DateTime(2021, 1, 1, 8, 0, 0) && meeting.End == new DateTime(2021, 1, 1, 9, 0, 0)
                    && meeting.Description is null && meeting.Participants.Count == 0, "Constructor of Meeting sets the values.");
                CheckAndWrite(() => owner.CalendarItems.Count == 0, "Constructor of Meeting does not add the meeting to the calendar of the owner.");
                meeting.Description = "Description";
                CheckAndWrite(() => meeting.Description == "Description", "CalendarItem.Description can be set.");
            }
            {
                Console.WriteLine("Checking CalendarItem.IsCollision");
                var owner = new User(username: "user1");
                CalendarItem item = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                CalendarItem overlapping = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 7, 0, 0), end: new DateTime(2021, 1, 1, 8, 30, 0));
                CalendarItem inside = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 15, 0), end: new DateTime(2021, 1, 1, 8, 45, 0));
                CalendarItem later = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                CalendarItem touching = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 9, 0, 0), end: new DateTime(2021, 1, 1, 10, 0, 0));
                CheckAndWrite(() => overlapping.IsCollision(item) && item.IsCollision(overlapping), "CalendarItem.IsCollision detects overlapping items.", 2);
                CheckAndWrite(() => inside.IsCollision(item) && item.IsCollision(inside), "CalendarItem.IsCollision detects an item inside another item.", 2);
                CheckAndWrite(() => item.IsCollision(item), "CalendarItem.IsCollision detects an item with the same time.");
                CheckAndWrite(() => !later.IsCollision(item) && !item.IsCollision(later), "CalendarItem.IsCollision returns false if the items do not overlap.", 2);
                CheckAndWrite(() => !touching.IsCollision(item) && !item.IsCollision(touching), "CalendarItem.IsCollision returns false if one item ends when the other begins.");
            }
            {
                Console.WriteLine("Checking User.TryAddItem");
                var owner = new User(username: "user1");
                CalendarItem item1 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                CheckAndWrite(() => owner.TryAddItem(item1) && owner.CalendarItems.Count == 1 && owner.CalendarItems[0] == item1, "User.TryAddItem adds the item to CalendarItems.", 2);

                CalendarItem item2 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 7, 0, 0), end: new DateTime(2021, 1, 1, 8, 30, 0));
                CheckAndWrite(() => !owner.TryAddItem(item2) && owner.CalendarItems.Count == 1, "User.TryAddItem rejects a collision.", 2);

                CalendarItem item3 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 9, 0, 0), end: new DateTime(2021, 1, 1, 10, 0, 0));
                CheckAndWrite(() => owner.TryAddItem(item3) && owner.CalendarItems.Count == 2, "User.TryAddItem adds an item that begins when another item ends.", 2);

                CalendarItem item4 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 15, 0), end: new DateTime(2021, 1, 1, 8, 45, 0));
                CheckAndWrite(() => !owner.TryAddItem(item4) && owner.CalendarItems.Count == 2, "User.TryAddItem compares the item with all items in the calendar.", 2);
                CheckAndWrite(() => !owner.TryAddItem(item3) && owner.CalendarItems.Count == 2, "User.TryAddItem rejects an item that is already in the calendar.");

                var user = new User(username: "user2");
                CalendarItem otherUsersItem = new PrivateItem(owner: user, title: "Title", begin: new DateTime(2021, 1, 1, 12, 0, 0), end: new DateTime(2021, 1, 1, 13, 0, 0));
                CheckAndWrite(() => !owner.TryAddItem(otherUsersItem) && owner.CalendarItems.Count == 2, "User.TryAddItem rejects a private item of another user.", 2);

                var otherUsersMeeting = new Meeting(owner: user, title: "Title", begin: new DateTime(2021, 1, 1, 14, 0, 0), end: new DateTime(2021, 1, 1, 15, 0, 0));
                CheckAndWrite(() => owner.TryAddItem(otherUsersMeeting) && owner.CalendarItems.Count == 3, "User.TryAddItem accepts a meeting of another user.", 2);
                CalendarItem item5 = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 14, 30, 0), end: new DateTime(2021, 1, 1, 15, 30, 0));
                CheckAndWrite(() => !owner.TryAddItem(item5) && owner.CalendarItems.Count == 3, "User.TryAddItem rejects an item that collides with a meeting.", 2);
            }
            {
                Console.WriteLine("Checking User.RemoveItem");
                var owner = new User(username: "user1");
                var item = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                var otherItem = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                owner.TryAddItem(item);
                owner.TryAddItem(otherItem);
                int oldCount = owner.CalendarItems.Count;
                owner.RemoveItem(otherItem);
                CheckAndWrite(() => oldCount == 2 && owner.CalendarItems.Count == 1 && owner.CalendarItems[0] == item, "User.RemoveItem removes only the passed item.", 2);
            }
            {
                Console.WriteLine("Checking Meeting.TryInvite");
                var owner = new User(username: "user1");
                var participant = new User(username: "user2");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(meeting);
                CheckAndWrite(() => meeting.TryInvite(participant) && meeting.Participants.Count == 1 && meeting.Participants[0] == participant, "Meeting.TryInvite adds the user to the participants.", 2);
                CheckAndWrite(() => participant.CalendarItems.Count == 1 && participant.CalendarItems[0] == meeting, "Meeting.TryInvite adds the meeting to the calendar of the user.", 2);
                CheckAndWrite(() => owner.CalendarItems.Count == 1, "Meeting.TryInvite does not change the calendar of the owner.");

                var busyUser = new User(username: "user3");
                busyUser.TryAddItem(new PrivateItem(owner: busyUser, title: "Title", begin: new DateTime(2021, 1, 1, 8, 30, 0), end: new DateTime(2021, 1, 1, 9, 30, 0)));
                CheckAndWrite(
                    () => !meeting.TryInvite(busyUser) && meeting.Participants.Count == 1 && busyUser.CalendarItems.Count == 1,
                    "Meeting.TryInvite returns false if the user is busy.", 2);

                CheckAndWrite(
                    () => !meeting.TryInvite(participant) && meeting.Participants.Count == 1 && participant.CalendarItems.Count == 1,
                    "Meeting.TryInvite returns false if the user is already invited.", 2);

                var participant2 = new User(username: "user4");
                meeting.TryInvite(participant2);
                meeting.RemoveParticipant(participant2);
                CheckAndWrite(() => meeting.Participants.Count == 1 && meeting.Participants[0] == participant, "Meeting.RemoveParticipant removes only the passed user.", 2);
            }
            {
                Console.WriteLine("Checking User.CancelMeeting (participant)");
                var owner = new User(username: "user1");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(meeting);
                var participant = new User(username: "user2");
                meeting.TryInvite(participant);
                var participant2 = new User(username: "user3");
                meeting.TryInvite(participant2);
                var privateItem = new PrivateItem(owner: participant, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                participant.TryAddItem(privateItem);

                int oldParticipantsCount = meeting.Participants.Count;
                int oldItemsCount = participant.CalendarItems.Count;
                CheckAndWrite(() =>
                {
                    participant.CancelMeeting(meeting);
                    return oldParticipantsCount == 2 && meeting.Participants.Count == 1 && meeting.Participants[0] == participant2;
                },
                    "User.CancelMeeting removes the participant from the meeting.", 2);
                CheckAndWrite(() =>
                    oldItemsCount == 2 && participant.CalendarItems.Count == 1 && participant.CalendarItems[0] == privateItem,
                    "User.CancelMeeting removes only the meeting from the calendar of the participant.", 2);
                CheckAndWrite(() => owner.CalendarItems.Count == 1, "User.CancelMeeting keeps the meeting in the calendar of the owner.", 2);
                CheckAndWrite(() => participant2.CalendarItems.Count == 1, "User.CancelMeeting keeps the meeting in the calendar of the other participant.", 2);
            }
            {
                Console.WriteLine("Checking User.CancelMeeting (owner)");
                var owner = new User(username: "user1");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                owner.TryAddItem(meeting);
                var ownersItem = new PrivateItem(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                owner.TryAddItem(ownersItem);
                var participant = new User(username: "user2");
                meeting.TryInvite(participant);
                var participant2 = new User(username: "user3");
                meeting.TryInvite(participant2);
                var participantsItem = new PrivateItem(owner: participant2, title: "Title", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                participant2.TryAddItem(participantsItem);
                int oldOwnerItemsCount = owner.CalendarItems.Count;
                int oldParticipantsItemsCount = participant.CalendarItems.Count + participant2.CalendarItems.Count;
                CheckAndWrite(() =>
                {
                    owner.CancelMeeting(meeting);
                    return oldOwnerItemsCount == 2 && owner.CalendarItems.Count == 1 && owner.CalendarItems[0] == ownersItem;
                },
                    "User.CancelMeeting removes the meeting from the calendar of the owner.", 2);
                CheckAndWrite(() =>
                    oldParticipantsItemsCount == 3 && participant.CalendarItems.Count == 0
                    && participant2.CalendarItems.Count == 1 && participant2.CalendarItems[0] == participantsItem,
                    "User.CancelMeeting removes the meeting from the calendars of all participants.", 2);
            }
            {
                Console.WriteLine("Checking User.CancelMeeting (several meetings)");
                var owner = new User(username: "user1");
                var meeting = new Meeting(owner: owner, title: "Title", begin: new DateTime(2021, 1, 1, 8, 0, 0), end: new DateTime(2021, 1, 1, 9, 0, 0));
                var otherMeeting = new Meeting(owner: owner, title: "Other", begin: new DateTime(2021, 1, 1, 10, 0, 0), end: new DateTime(2021, 1, 1, 11, 0, 0));
                owner.TryAddItem(meeting);
                owner.TryAddItem(otherMeeting);
                var participant = new User(username: "user2");
                meeting.TryInvite(participant);
                otherMeeting.TryInvite(participant);
                CheckAndWrite(() =>
                {
                    participant.CancelMeeting(meeting);
                    return participant.CalendarItems.Count == 1 && participant.CalendarItems[0] == otherMeeting;
                }, "User.CancelMeeting removes only the passed meeting from the calendar of the participant.", 2);
                CheckAndWrite(() =>
                {
                    owner.CancelMeeting(meeting);
                    return owner.CalendarItems.Count == 1 && owner.CalendarItems[0] == otherMeeting;
                }, "User.CancelMeeting removes only the passed meeting from the calendar of the owner.", 2);
            }

            WriteSummary();
        }
    }
}
