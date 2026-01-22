namespace Fcg.Contracts.Payments;

public record PaymentRejectedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount,
    Guid? GameId,
    DateTime RejectedAt,
    string Reason);
