namespace PowerbankCheck.Application;

/// <summary>
/// The result of a baggage check.
/// </summary>
public enum CheckResult
{
    /// <summary>The bag may be taken on board.</summary>
    Allowed,

    /// <summary>The bag may be taken on board if the airline approves it.</summary>
    ApprovalRequired,

    /// <summary>The bag must not be taken on board.</summary>
    Forbidden
}
