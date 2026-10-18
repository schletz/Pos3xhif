using System;
using System.Collections.Generic;
using System.Linq;
using Fitnesscenter.Dtos;
using Fitnesscenter.Infrastructure;
using Fitnesscenter.Model;
using Microsoft.EntityFrameworkCore;

namespace Fitnesscenter.Services;

/// <summary>
/// Queries and use cases of the fitness center. The context is provided via dependency injection.
/// </summary>
public class FitnessService
{
    private readonly FitnessContext _db;

    public FitnessService(FitnessContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all members with <see cref="Member.IsActive"/> set to true.
    /// </summary>
    public List<ActiveMemberDto> GetActiveMembers()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all participations of the member with the given id.
    /// Returns an empty list if there are none.
    /// </summary>
    public List<Participation> GetParticipationsOfMember(int memberId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all trainers who hold at least one training session on the given date.
    /// </summary>
    public List<Trainer> GetTrainerWithTrainingSessionsOnDate(DateOnly date)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all members who have not registered for any training session.
    /// </summary>
    public List<Member> GetMembersWithoutParticipation()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all training sessions with at least one participant of the given membership type.
    /// </summary>
    public List<TrainingSession> GetSessionsWithMembershipType(string membershipType)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all training sessions whose number of participations equals
    /// <see cref="TrainingSession.MaxParticipants"/>.
    /// </summary>
    public List<TrainingSession> GetFullTrainingSessions()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all training sessions with room, trainer and the number of participations.
    /// </summary>
    public List<TrainingSessionWithCountDto> GetTrainingSessionsWithParticipantCounts()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the member with the given id and all of his or her visits.
    /// Returns null if the member does not exist.
    /// </summary>
    public MemberWithVisitsDto? GetMemberWithVisits(int memberId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns every trainer with the number of ratings with the value 5
    /// over all of his or her training sessions.
    /// </summary>
    public List<MaxRatingCountPerTrainerDto> GetMaxRatingCountPerTrainer()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all trainers who received exactly <paramref name="count"/> ratings with the value 1
    /// over all of their training sessions.
    /// </summary>
    public List<Trainer> GetTrainersWithMinimalRatingCount(int count)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all rooms with the training sessions that start between
    /// <paramref name="timeFrom"/> and <paramref name="timeTo"/> (both inclusive).
    /// </summary>
    public List<RoomWithTrainingSessionsDto> GetRoomsWithTrainingSessions(DateTime timeFrom, DateTime timeTo)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Registers a member for a training session. The new participation has no rating.
    /// </summary>
    /// <returns>The new participation.</returns>
    /// <exception cref="FitnessServiceException">The member or the training session does not exist,
    /// the member is already registered or the training session is full.</exception>
    public Participation RegisterMemberToTrainingSession(int memberId, int trainingSessionId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the rating of the participation of a member in a training session.
    /// </summary>
    /// <exception cref="FitnessServiceException">The rating is not between 1 and 5
    /// or the participation does not exist.</exception>
    public void UpdateRating(int memberId, int trainingSessionId, int rating)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Closes all open visits of the member and adds a new open visit that starts at
    /// <paramref name="dateTime"/>.
    /// </summary>
    /// <returns>The new visit.</returns>
    /// <exception cref="FitnessServiceException">The member does not exist.</exception>
    public Visit AddVisit(int memberId, DateTime dateTime)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds a new training session for the trainer in the given room.
    /// </summary>
    /// <returns>The new training session.</returns>
    /// <exception cref="FitnessServiceException">The time is outside the opening hours,
    /// there are less than 3 participants allowed or the time is not after the last
    /// training session of the trainer.</exception>
    public TrainingSession AddTrainingSession(
        Trainer trainer, Room room, string type, DateTime time,
        int durationMinutes, int maxParticipants)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }
}
