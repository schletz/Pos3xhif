using System;

namespace Fitnesscenter.Services;

/// <summary>
/// Thrown by <see cref="FitnessService"/> when a business rule is violated.
/// </summary>
public class FitnessServiceException : Exception
{
    public FitnessServiceException()
    {
    }

    public FitnessServiceException(string? message) : base(message)
    {
    }

    public FitnessServiceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
