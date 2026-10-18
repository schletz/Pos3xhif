using System;

namespace BusService.Application.Services;

/// <summary>
/// Thrown when a business rule of <see cref="DispatchService"/> is violated.
/// </summary>
public class DispatchServiceException : Exception
{
    public DispatchServiceException(string message) : base(message)
    { }
}
