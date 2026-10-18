namespace Fitnesscenter.Dtos;

/// <summary>
/// An active member with id, name and email address.
/// </summary>
public record ActiveMemberDto(int Id, string FirstName, string LastName, string Email);
