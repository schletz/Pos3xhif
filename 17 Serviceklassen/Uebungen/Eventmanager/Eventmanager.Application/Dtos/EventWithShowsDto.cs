using System.Collections.Generic;

namespace Eventmanager.Application.Dtos;

public record EventWithShowsDto(int EventId, string EventName, List<ShowDto> Shows);
