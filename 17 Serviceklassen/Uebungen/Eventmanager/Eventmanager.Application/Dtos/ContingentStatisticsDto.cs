using System;

namespace Eventmanager.Application.Dtos;

/// <summary>
/// Sold and reserved persons (guest + pax) of a contingent.
/// </summary>
public record ContingentStatisticsDto(int ContingentId, DateTime ShowDate, int SoldPersons, int ReservedPersons);
