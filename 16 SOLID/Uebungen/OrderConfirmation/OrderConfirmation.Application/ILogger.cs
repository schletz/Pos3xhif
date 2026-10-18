namespace OrderConfirmation.Application;

/// <summary>
/// Writes log messages, e.g. to the console or to a file.
/// </summary>
public interface ILogger
{
    void Log(string message);
}
