namespace Fcg.Contracts.Payments;

public record PaymentApprovedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount,
    Guid? GameId,
    DateTime ApprovedAt);
