namespace Fcg.Contracts.Payments;

/// <summary>
/// Event published when payment processing starts.
/// </summary>
public record PaymentProcessingStartedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount,
    Guid? GameId,
    DateTime StartedAt);

/// <summary>
/// Event published when payment fails.
/// </summary>
public record PaymentFailedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount,
    string Reason,
    DateTime FailedAt);
