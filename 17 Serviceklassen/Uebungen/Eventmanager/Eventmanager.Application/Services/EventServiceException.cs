using System;

namespace Eventmanager.Application.Services;

/// <summary>
/// Thrown when a business rule of <see cref="EventService"/> is violated.
/// </summary>
public class EventServiceException : Exception
{
    public EventServiceException(string message) : base(message)
    { }
}
