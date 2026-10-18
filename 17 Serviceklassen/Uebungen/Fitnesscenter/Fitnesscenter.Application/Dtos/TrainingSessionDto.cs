using System;

namespace Fitnesscenter.Dtos;

/// <summary>
/// A training session with the trainer and the start time.
/// </summary>
public record TrainingSessionDto(int TrainerId, string TrainerFirstName, string TrainerLastName, DateTime Time);
