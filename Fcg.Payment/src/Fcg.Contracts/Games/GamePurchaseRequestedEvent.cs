namespace Fcg.Contracts.Games;

public record GamePurchaseRequestedEvent(
    Guid CorrelationId,
    Guid UserId,
    Guid GameId,
    DateTime RequestedAt);
