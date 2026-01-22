using Fcg.Contracts.Games;
using Fcg.Payment.Domain.Repositories;
using MassTransit;

namespace Fcg.Payment.Application.Consumers;

public class GamePurchaseRejectedConsumer : IConsumer<GamePurchaseRejectedEvent>
{
    private readonly IPaymentRepository _payments;

    public GamePurchaseRejectedConsumer(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task Consume(ConsumeContext<GamePurchaseRejectedEvent> context)
    {
        // CorrelationId == PaymentId (simplificação)
        var payment = await _payments.GetByIdAsync(context.Message.CorrelationId, context.CancellationToken);

        if (payment is null)
            return;

        if (payment.Status != Domain.Enums.PaymentStatus.Pending)
            return;

        payment.Reject();
        await _payments.UpdateAsync(payment, context.CancellationToken);
    }
}
