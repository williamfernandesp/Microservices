namespace Fcg.Contracts.Notifications;

/// <summary>
/// Event published when a user notification should be sent.
/// </summary>
public record UserNotificationEvent(
    Guid UserId,
    string Title,
    string Message,
    NotificationType Type,
    DateTime CreatedAt);

public enum NotificationType
{
    PaymentApproved,
    PaymentRejected,
    GamePurchased,
    PromotionAvailable,
    SystemAlert
}
