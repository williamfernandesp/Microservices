namespace Fcg.Contracts.Games;

public record GamePurchaseValidatedEvent(
    Guid CorrelationId,
    Guid UserId,
    Guid GameId,
    decimal Price,
    DateTime ValidatedAt);
