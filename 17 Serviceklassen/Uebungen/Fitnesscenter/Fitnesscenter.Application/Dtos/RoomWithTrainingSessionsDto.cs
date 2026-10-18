using System.Collections.Generic;

namespace Fitnesscenter.Dtos;

/// <summary>
/// A room with its training sessions.
/// </summary>
public record RoomWithTrainingSessionsDto(int RoomId, string RoomName, List<TrainingSessionDto> Sessions);
