namespace Fitnesscenter.Dtos;

/// <summary>
/// A trainer with the number of ratings with the value 5 he or she received.
/// </summary>
public record MaxRatingCountPerTrainerDto(int Id, string FirstName, string LastName, int MaxRatingCount);
