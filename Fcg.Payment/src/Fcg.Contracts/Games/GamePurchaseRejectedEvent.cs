namespace Fcg.Contracts.Games;

public record GamePurchaseRejectedEvent(
    Guid CorrelationId,
    Guid UserId,
    Guid GameId,
    string Reason,
    DateTime RejectedAt);
