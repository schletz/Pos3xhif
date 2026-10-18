// *************************************************************************************************
// Tests for FitnessService. DO NOT CHANGE THIS FILE.
// Every test uses a new in-memory database with the same sample data (FitnessContext.Seed).
// Expected values are determined with SQL only, so the LINQ solution of the service methods
// cannot be read from this class.
// *************************************************************************************************
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Fitnesscenter.Dtos;
using Fitnesscenter.Infrastructure;
using Fitnesscenter.Model;
using Fitnesscenter.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fitnesscenter.Tests;

public class FitnessServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly FitnessContext _db;
    private readonly FitnessService _service;

    public FitnessServiceTests()
    {
        // EF Core would close a connection it manages itself after EnsureCreated, and the
        // in-memory database would be lost. Therefore the open connection is passed in.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new FitnessContext(options);
        _db.Database.EnsureCreated();
        _db.Seed();
        // The service must load its data from the database, not from entities tracked by the seed.
        _db.ChangeTracker.Clear();
        _service = new FitnessService(_db);
    }

    [Fact(DisplayName = "GetActiveMembers returns all active members.")]
    public void GetActiveMembers_ReturnsActiveMembers()
    {
        var expected = QueryDatabase(
            "SELECT id, firstName, lastName, email FROM member WHERE isActive = 1",
            r => new ActiveMemberDto(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3)));

        AssertSameSet(expected, _service.GetActiveMembers(), "GetActiveMembers()");
    }

    [Fact(DisplayName = "GetParticipationsOfMember returns all participations of the member.")]
    public void GetParticipationsOfMember_ReturnsParticipationsOfMember()
    {
        // 999 does not exist and must return an empty list.
        foreach (var memberId in QueryIds("SELECT id FROM member").Append(999))
        {
            var expected = QueryIds("SELECT id FROM participation WHERE memberId = $id", ("$id", memberId));
            var actual = _service.GetParticipationsOfMember(memberId).Select(p => p.Id);
            AssertSameIds(expected, actual, $"GetParticipationsOfMember({memberId})");
        }
    }

    [Fact(DisplayName = "GetTrainerWithTrainingSessionsOnDate returns all trainers with a training session on the date.")]
    public void GetTrainerWithTrainingSessionsOnDate_ReturnsTrainersOfDate()
    {
        var dates = QueryDatabase("SELECT DISTINCT date(time) FROM trainingSession", r => DateOnly.Parse(r.GetString(0)))
            .Append(new DateOnly(2024, 1, 1));
        foreach (var date in dates)
        {
            var expected = QueryIds(@"
                SELECT t.id FROM trainer t
                WHERE EXISTS (SELECT 1 FROM trainingSession ts WHERE ts.trainerId = t.id AND date(ts.time) = $date)",
                ("$date", date));
            var actual = _service.GetTrainerWithTrainingSessionsOnDate(date).Select(t => t.Id);
            AssertSameIds(expected, actual, $"GetTrainerWithTrainingSessionsOnDate({date:yyyy-MM-dd})");
        }
    }

    [Fact(DisplayName = "GetMembersWithoutParticipation returns all members without a participation.")]
    public void GetMembersWithoutParticipation_ReturnsMembersWithoutParticipation()
    {
        var expected = QueryIds(@"
            SELECT m.id FROM member m
            WHERE NOT EXISTS (SELECT 1 FROM participation p WHERE p.memberId = m.id)");
        var actual = _service.GetMembersWithoutParticipation().Select(m => m.Id);
        AssertSameIds(expected, actual, "GetMembersWithoutParticipation()");
    }

    [Fact(DisplayName = "GetSessionsWithMembershipType returns all training sessions with a participant of the membership type.")]
    public void GetSessionsWithMembershipType_ReturnsSessionsOfMembershipType()
    {
        // "Gold" does not exist and must return an empty list.
        var membershipTypes = QueryDatabase("SELECT DISTINCT membershipType FROM member", r => r.GetString(0))
            .Append("Gold");
        foreach (var membershipType in membershipTypes)
        {
            var expected = QueryIds(@"
                SELECT ts.id FROM trainingSession ts
                WHERE EXISTS (SELECT 1 FROM participation p INNER JOIN member m ON p.memberId = m.id
                              WHERE p.trainingSessionId = ts.id AND m.membershipType = $type)",
                ("$type", membershipType));
            var actual = _service.GetSessionsWithMembershipType(membershipType).Select(ts => ts.Id);
            AssertSameIds(expected, actual, $"GetSessionsWithMembershipType(\"{membershipType}\")");
        }
    }

    [Fact(DisplayName = "GetFullTrainingSessions returns all training sessions without a free place.")]
    public void GetFullTrainingSessions_ReturnsFullSessions()
    {
        var expected = QueryIds(@"
            SELECT ts.id FROM trainingSession ts
            WHERE ts.maxParticipants = (SELECT COUNT(*) FROM participation p WHERE p.trainingSessionId = ts.id)");
        var actual = _service.GetFullTrainingSessions().Select(ts => ts.Id);
        AssertSameIds(expected, actual, "GetFullTrainingSessions()");
    }

    [Fact(DisplayName = "GetTrainingSessionsWithParticipantCounts returns all training sessions with the number of participants.")]
    public void GetTrainingSessionsWithParticipantCounts_ReturnsSessionsWithCount()
    {
        var expected = QueryDatabase(@"
            SELECT ts.id, r.name, ts.time, t.firstName, t.lastName,
                   (SELECT COUNT(*) FROM participation p WHERE p.trainingSessionId = ts.id)
            FROM trainingSession ts
            INNER JOIN room r ON ts.roomId = r.id
            INNER JOIN trainer t ON ts.trainerId = t.id",
            r => new TrainingSessionWithCountDto(
                r.GetInt32(0), r.GetString(1), r.GetDateTime(2), r.GetString(3), r.GetString(4), r.GetInt32(5)));

        AssertSameSet(expected, _service.GetTrainingSessionsWithParticipantCounts(), "GetTrainingSessionsWithParticipantCounts()");
    }

    [Fact(DisplayName = "GetMemberWithVisits returns the member with all visits or null for an unknown member.")]
    public void GetMemberWithVisits_ReturnsMemberWithVisits()
    {
        foreach (var memberId in QueryIds("SELECT id FROM member"))
        {
            var expected = QueryDatabase(
                "SELECT id, firstName, lastName, email FROM member WHERE id = $id",
                r => (Id: r.GetInt32(0), FirstName: r.GetString(1), LastName: r.GetString(2), Email: r.GetString(3)),
                ("$id", memberId)).Single();
            var expectedVisits = QueryDatabase(
                "SELECT start, end FROM visit WHERE memberId = $id",
                r => new VisitDto(r.GetDateTime(0), r.IsDBNull(1) ? null : r.GetDateTime(1)),
                ("$id", memberId));

            var actual = _service.GetMemberWithVisits(memberId);
            Assert.True(actual is not null, $"GetMemberWithVisits({memberId}) returned null.");
            Assert.True(
                expected == (actual.Id, actual.FirstName, actual.LastName, actual.Email),
                $"GetMemberWithVisits({memberId}) failed. Expected {expected}, got {actual}.");
            AssertSameSet(expectedVisits, actual.Visits, $"GetMemberWithVisits({memberId}).Visits");
        }
        Assert.True(_service.GetMemberWithVisits(999) is null, "GetMemberWithVisits(999) must return null.");
    }

    [Fact(DisplayName = "GetMaxRatingCountPerTrainer returns every trainer with the number of ratings with the value 5.")]
    public void GetMaxRatingCountPerTrainer_ReturnsCountOfRating5()
    {
        var expected = QueryDatabase(@"
            SELECT t.id, t.firstName, t.lastName,
                   (SELECT COUNT(*) FROM trainingSession ts INNER JOIN participation p ON p.trainingSessionId = ts.id
                    WHERE ts.trainerId = t.id AND p.rating = 5)
            FROM trainer t",
            r => new MaxRatingCountPerTrainerDto(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetInt32(3)));

        AssertSameSet(expected, _service.GetMaxRatingCountPerTrainer(), "GetMaxRatingCountPerTrainer()");
    }

    [Fact(DisplayName = "GetTrainersWithMinimalRatingCount returns all trainers with exactly count ratings with the value 1.")]
    public void GetTrainersWithMinimalRatingCount_ReturnsTrainersWithCount()
    {
        foreach (var count in Enumerable.Range(0, 6))
        {
            var expected = QueryIds(@"
                SELECT t.id FROM trainer t
                WHERE (SELECT COUNT(*) FROM trainingSession ts INNER JOIN participation p ON ts.id = p.trainingSessionId
                       WHERE ts.trainerId = t.id AND p.rating = 1) = $count",
                ("$count", count));
            var actual = _service.GetTrainersWithMinimalRatingCount(count).Select(t => t.Id);
            AssertSameIds(expected, actual, $"GetTrainersWithMinimalRatingCount({count})");
        }
    }

    [Fact(DisplayName = "GetRoomsWithTrainingSessions returns all rooms with the training sessions in the period.")]
    public void GetRoomsWithTrainingSessions_ReturnsRoomsWithSessionsInPeriod()
    {
        // The third period starts and ends exactly at the time of a training session (both inclusive).
        var sessionTimes = QueryDatabase("SELECT time FROM trainingSession ORDER BY time", r => r.GetDateTime(0));
        var periods = new List<(DateTime From, DateTime To)>
        {
            (new DateTime(2025, 2, 1), new DateTime(2025, 3, 1)),
            (new DateTime(2025, 3, 1), new DateTime(2025, 4, 1)),
            (sessionTimes[2], sessionTimes[5])
        };
        var rooms = QueryDatabase("SELECT id, name FROM room", r => (Id: r.GetInt32(0), Name: r.GetString(1)));

        foreach (var (from, to) in periods)
        {
            var method = $"GetRoomsWithTrainingSessions({from:s}, {to:s})";
            var actual = _service.GetRoomsWithTrainingSessions(from, to);
            Assert.True(actual.Count == rooms.Count, $"{method} row count failed: expected {rooms.Count}, got {actual.Count}.");
            foreach (var room in rooms)
            {
                var actualRoom = actual.FirstOrDefault(r => r.RoomId == room.Id);
                Assert.True(actualRoom is not null, $"{method}: room {room.Id} not found in your result.");
                Assert.True(actualRoom.RoomName == room.Name, $"{method}: wrong name of room {room.Id}.");
                var expectedSessions = QueryDatabase(@"
                    SELECT t.id, t.firstName, t.lastName, ts.time
                    FROM trainingSession ts INNER JOIN trainer t ON ts.trainerId = t.id
                    WHERE ts.roomId = $roomId AND ts.time >= $from AND ts.time <= $to",
                    r => new TrainingSessionDto(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetDateTime(3)),
                    ("$roomId", room.Id), ("$from", from), ("$to", to));
                AssertSameSet(expectedSessions, actualRoom.Sessions, $"{method}, room {room.Id}");
            }
        }
    }

    [Fact(DisplayName = "RegisterMemberToTrainingSession adds a participation without a rating.")]
    public void RegisterMemberToTrainingSession_AddsParticipation()
    {
        var (memberId, sessionId) = QueryDatabase(@"
            SELECT m.id, ts.id FROM member m CROSS JOIN trainingSession ts
            WHERE NOT EXISTS (SELECT 1 FROM participation p WHERE p.memberId = m.id)
              AND ts.maxParticipants > (SELECT COUNT(*) FROM participation p WHERE p.trainingSessionId = ts.id)
            ORDER BY m.id, ts.id LIMIT 1",
            r => (r.GetInt32(0), r.GetInt32(1))).Single();

        var participation = _service.RegisterMemberToTrainingSession(memberId, sessionId);

        var stored = QueryDatabase(
            "SELECT id, rating FROM participation WHERE memberId = $memberId AND trainingSessionId = $sessionId",
            r => (Id: r.GetInt32(0), HasRating: !r.IsDBNull(1)),
            ("$memberId", memberId), ("$sessionId", sessionId));
        Assert.True(stored.Count == 1, $"Expected 1 participation of member {memberId} in training session {sessionId}, found {stored.Count}.");
        Assert.True(participation.Id == stored[0].Id, "The returned participation must be the stored participation.");
        Assert.False(stored[0].HasRating, "A new participation must not have a rating.");
    }

    [Fact(DisplayName = "RegisterMemberToTrainingSession throws 'Member not found.' for an unknown member.")]
    public void RegisterMemberToTrainingSession_UnknownMember_Throws()
    {
        var sessionId = QueryIds(@"
            SELECT ts.id FROM trainingSession ts
            WHERE ts.maxParticipants > (SELECT COUNT(*) FROM participation p WHERE p.trainingSessionId = ts.id)").First();
        AssertThrowsWithMessage(() => _service.RegisterMemberToTrainingSession(999, sessionId), "Member not found.");
    }

    [Fact(DisplayName = "RegisterMemberToTrainingSession throws 'TrainingSession not found.' for an unknown training session.")]
    public void RegisterMemberToTrainingSession_UnknownSession_Throws()
    {
        var memberId = QueryIds("SELECT id FROM member").First();
        AssertThrowsWithMessage(() => _service.RegisterMemberToTrainingSession(memberId, 999), "TrainingSession not found.");
    }

    [Fact(DisplayName = "RegisterMemberToTrainingSession throws 'Member already registered.' for a second registration.")]
    public void RegisterMemberToTrainingSession_AlreadyRegistered_Throws()
    {
        // A training session with free places, so only the rule "already registered" is violated.
        var (memberId, sessionId) = QueryDatabase(@"
            SELECT p.memberId, p.trainingSessionId FROM participation p INNER JOIN trainingSession ts ON p.trainingSessionId = ts.id
            WHERE ts.maxParticipants > (SELECT COUNT(*) FROM participation p2 WHERE p2.trainingSessionId = ts.id)
            ORDER BY p.id LIMIT 1",
            r => (r.GetInt32(0), r.GetInt32(1))).Single();
        AssertThrowsWithMessage(() => _service.RegisterMemberToTrainingSession(memberId, sessionId), "Member already registered.");
    }

    [Fact(DisplayName = "RegisterMemberToTrainingSession throws 'Training Session is already full.' for a full training session.")]
    public void RegisterMemberToTrainingSession_FullSession_Throws()
    {
        var (memberId, sessionId) = QueryDatabase(@"
            SELECT m.id, ts.id FROM member m CROSS JOIN trainingSession ts
            WHERE ts.maxParticipants = (SELECT COUNT(*) FROM participation p WHERE p.trainingSessionId = ts.id)
              AND NOT EXISTS (SELECT 1 FROM participation p WHERE p.trainingSessionId = ts.id AND p.memberId = m.id)
            ORDER BY m.id, ts.id LIMIT 1",
            r => (r.GetInt32(0), r.GetInt32(1))).Single();
        AssertThrowsWithMessage(() => _service.RegisterMemberToTrainingSession(memberId, sessionId), "Training Session is already full.");
    }

    [Fact(DisplayName = "UpdateRating stores the rating of the participation.")]
    public void UpdateRating_StoresRating()
    {
        var participations = QueryDatabase(
            "SELECT id, memberId, trainingSessionId FROM participation WHERE rating IS NULL OR rating <> 4 ORDER BY id LIMIT 2",
            r => (Id: r.GetInt32(0), MemberId: r.GetInt32(1), SessionId: r.GetInt32(2)));
        var (changed, unchanged) = (participations[0], participations[1]);
        var unchangedRating = QueryDatabase("SELECT rating FROM participation WHERE id = $id",
            r => r.IsDBNull(0) ? null as int? : r.GetInt32(0), ("$id", unchanged.Id)).Single();

        _service.UpdateRating(changed.MemberId, changed.SessionId, 4);

        var ratings = QueryDatabase("SELECT id, rating FROM participation WHERE id IN ($id1, $id2)",
            r => (Id: r.GetInt32(0), Rating: r.IsDBNull(1) ? null as int? : r.GetInt32(1)),
            ("$id1", changed.Id), ("$id2", unchanged.Id));
        Assert.True(ratings.Single(r => r.Id == changed.Id).Rating == 4,
            $"Rating of member {changed.MemberId} in training session {changed.SessionId} must be 4.");
        Assert.True(ratings.Single(r => r.Id == unchanged.Id).Rating == unchangedRating,
            "UpdateRating must not change other participations.");
    }

    [Theory(DisplayName = "UpdateRating throws 'Rating must be between 1 and 5.' for an invalid rating.")]
    [InlineData(0)]
    [InlineData(6)]
    public void UpdateRating_InvalidRating_Throws(int rating)
    {
        var (memberId, sessionId) = QueryDatabase("SELECT memberId, trainingSessionId FROM participation ORDER BY id LIMIT 1",
            r => (r.GetInt32(0), r.GetInt32(1))).Single();
        AssertThrowsWithMessage(() => _service.UpdateRating(memberId, sessionId, rating), "Rating must be between 1 and 5.");
    }

    [Fact(DisplayName = "UpdateRating throws 'Participation not found.' if the member did not take part.")]
    public void UpdateRating_UnknownParticipation_Throws()
    {
        var (memberId, sessionId) = QueryDatabase(@"
            SELECT m.id, ts.id FROM member m CROSS JOIN trainingSession ts
            WHERE NOT EXISTS (SELECT 1 FROM participation p WHERE p.trainingSessionId = ts.id AND p.memberId = m.id)
            ORDER BY m.id, ts.id LIMIT 1",
            r => (r.GetInt32(0), r.GetInt32(1))).Single();
        AssertThrowsWithMessage(() => _service.UpdateRating(memberId, sessionId, 4), "Participation not found.");
    }

    [Fact(DisplayName = "AddVisit closes all open visits of the member and adds an open visit.")]
    public void AddVisit_ClosesOpenVisitsAndAddsVisit()
    {
        var checkIn = new DateTime(2026, 2, 26, 14, 0, 0);
        foreach (var memberId in QueryIds("SELECT id FROM member"))
        {
            var openOfMemberBefore = CountVisits("memberId = $id AND end IS NULL", memberId);
            var openOfOthersBefore = CountVisits("memberId <> $id AND end IS NULL", memberId);

            var visit = _service.AddVisit(memberId, checkIn);

            var newVisits = QueryIds("SELECT id FROM visit WHERE memberId = $id AND start = $start AND end IS NULL",
                ("$id", memberId), ("$start", checkIn));
            Assert.True(newVisits.Count == 1 && newVisits[0] == visit.Id,
                $"AddVisit({memberId}): the returned visit must be stored with start {checkIn:s} and no end.");
            Assert.True(CountVisits("memberId = $id AND end IS NULL", memberId) == 1,
                $"AddVisit({memberId}): the member must have exactly 1 open visit (the new one).");
            Assert.True(CountVisits("memberId = $id AND end = $end", memberId, checkIn) == openOfMemberBefore,
                $"AddVisit({memberId}): all {openOfMemberBefore} open visits must end at {checkIn:s}.");
            Assert.True(CountVisits("memberId <> $id AND end IS NULL", memberId) == openOfOthersBefore,
                $"AddVisit({memberId}): visits of other members must not change.");
        }
    }

    [Fact(DisplayName = "AddVisit throws 'Invalid member id.' for an unknown member.")]
    public void AddVisit_UnknownMember_Throws()
    {
        AssertThrowsWithMessage(() => _service.AddVisit(999, new DateTime(2026, 2, 26, 14, 0, 0)), "Invalid member id.");
    }

    [Fact(DisplayName = "AddTrainingSession stores the training session if all rules are met.")]
    public void AddTrainingSession_StoresSession()
    {
        var lastSessionOfTrainer = LastSessionTimes();
        var earlyTrainer = FindTrainer(lastSessionOfTrainer[0].TrainerId);
        var lateTrainer = FindTrainer(lastSessionOfTrainer[^1].TrainerId);
        var room = FirstRoom();
        var lastDay = lastSessionOfTrainer[^1].Time.Date;
        var nextDay = lastDay.AddDays(1);

        var newSessions = new[]
        {
            // Only the last training session of the same trainer counts. This time is before
            // the last training session of lateTrainer, but after the one of earlyTrainer.
            (Trainer: earlyTrainer, Time: lastDay.AddHours(8), Duration: 30),
            // 8:00 is the start of the opening hours, a session from 19:00 to 20:00 ends with them.
            (Trainer: lateTrainer, Time: nextDay.AddHours(8), Duration: 60),
            (Trainer: lateTrainer, Time: nextDay.AddHours(19), Duration: 60)
        };
        var sessions = new List<(int TrainerId, DateTime Time, int Duration, int Id)>();
        foreach (var (trainer, time, duration) in newSessions)
        {
            var session = _service.AddTrainingSession(trainer, room, "Ausdauer", time, duration, 3);
            sessions.Add((trainer.Id, time, duration, session.Id));
        }

        foreach (var session in sessions)
        {
            var found = QueryIds(@"
                SELECT id FROM trainingSession
                WHERE id = $id AND trainerId = $trainerId AND roomId = $roomId AND time = $time
                  AND type = 'Ausdauer' AND durationMinutes = $duration AND maxParticipants = 3",
                ("$id", session.Id), ("$trainerId", session.TrainerId), ("$roomId", room.Id),
                ("$time", session.Time), ("$duration", session.Duration));
            Assert.True(found.Count == 1, $"Training session of trainer {session.TrainerId} at {session.Time:s} not found in the database.");
        }
    }

    [Theory(DisplayName = "AddTrainingSession throws an exception if a rule is violated.")]
    [InlineData(4, 0, 60, 5, "Invalid time.")]
    [InlineData(19, 30, 60, 5, "Invalid time.")]
    [InlineData(8, 0, 60, 2, "Invalid max participants.")]
    public void AddTrainingSession_InvalidValues_Throws(int hour, int minute, int duration, int maxParticipants, string message)
    {
        // A day after all training sessions, so only the rule in the test data is violated.
        var lastSessionOfTrainer = LastSessionTimes();
        var trainer = FindTrainer(lastSessionOfTrainer[0].TrainerId);
        var room = FirstRoom();
        var time = lastSessionOfTrainer[^1].Time.Date.AddDays(1).AddHours(hour).AddMinutes(minute);

        AssertThrowsWithMessage(
            () => _service.AddTrainingSession(trainer, room, "Ausdauer", time, duration, maxParticipants), message);
    }

    [Fact(DisplayName = "AddTrainingSession throws 'Time must be after the last training session.' for an earlier time.")]
    public void AddTrainingSession_BeforeLastSession_Throws()
    {
        var lastSessionOfTrainer = LastSessionTimes();
        var (trainerId, lastTime) = lastSessionOfTrainer[^1];
        var trainer = FindTrainer(trainerId);
        var room = FirstRoom();
        // Within the opening hours, but before the last training session of the trainer.
        var time = lastTime.Date.AddDays(-1).AddHours(10);

        AssertThrowsWithMessage(
            () => _service.AddTrainingSession(trainer, room, "Ausdauer", time, 30, 3),
            "Time must be after the last training session.");
    }

    /// <summary>
    /// Returns the time of the last training session of every trainer, the earliest one first.
    /// </summary>
    private List<(int TrainerId, DateTime Time)> LastSessionTimes() => QueryDatabase(
        "SELECT trainerId, MAX(time) AS lastTime FROM trainingSession GROUP BY trainerId ORDER BY lastTime",
        r => (r.GetInt32(0), r.GetDateTime(1)));

    private Trainer FindTrainer(int id) => _db.Trainers.Single(t => t.Id == id);

    private Room FirstRoom() => _db.Rooms.OrderBy(r => r.Id).First();

    private int CountVisits(string condition, int memberId, DateTime? end = null) => QueryDatabase(
        $"SELECT COUNT(*) FROM visit WHERE {condition}", r => r.GetInt32(0),
        ("$id", memberId), ("$end", end ?? DateTime.MinValue)).Single();

    private static void AssertThrowsWithMessage(Action action, string message)
    {
        var ex = Assert.Throws<FitnessServiceException>(action);
        Assert.True(ex.Message == message, $"Wrong exception message. Expected '{message}', got '{ex.Message}'.");
    }

    private static void AssertSameSet<T>(IReadOnlyCollection<T> expected, IReadOnlyCollection<T> actual, string method)
    {
        Assert.True(expected.Count == actual.Count, $"{method} row count failed: expected {expected.Count}, got {actual.Count}.");
        foreach (var item in expected)
            Assert.True(actual.Contains(item), $"{method}: row not found in your result: {item}.");
    }

    private static void AssertSameIds(IEnumerable<int> expected, IEnumerable<int> actual, string method)
    {
        var expectedIds = expected.Order().ToList();
        var actualIds = actual.Order().ToList();
        Assert.True(expectedIds.SequenceEqual(actualIds),
            $"{method} failed. Expected ids [{string.Join(", ", expectedIds)}], got [{string.Join(", ", actualIds)}].");
    }

    private List<int> QueryIds(string sql, params (string Name, object Value)[] parameters) =>
        QueryDatabase(sql, r => r.GetInt32(0), parameters);

    private List<T> QueryDatabase<T>(string sql, Func<DbDataReader, T> projection, params (string Name, object Value)[] parameters)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
            command.Parameters.AddWithValue(name, value);
        using var reader = command.ExecuteReader();
        var rows = new List<T>();
        while (reader.Read())
            rows.Add(projection(reader));
        return rows;
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
