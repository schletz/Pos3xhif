using System.Collections.Generic;

namespace Fitnesscenter.Dtos;

/// <summary>
/// A member with all of his or her visits.
/// </summary>
public record MemberWithVisitsDto(int Id, string FirstName, string LastName, string Email, List<VisitDto> Visits);
