using System;

namespace Fitnesscenter.Dtos;

/// <summary>
/// A training session with room, time, trainer and the number of participations.
/// </summary>
public record TrainingSessionWithCountDto(
    int Id, string RoomName, DateTime DateTime,
    string TrainerFirstName, string TrainerLastName, int ParticipantCount);
