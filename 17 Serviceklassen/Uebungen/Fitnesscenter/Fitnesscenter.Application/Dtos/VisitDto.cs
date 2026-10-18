using System;

namespace Fitnesscenter.Dtos;

/// <summary>
/// Start and end of a visit. End is null while the visit is still open.
/// </summary>
public record VisitDto(DateTime Start, DateTime? End);
