using Fcg.Contracts.Games;
using Fcg.Payment.Domain.Repositories;
using MassTransit;

namespace Fcg.Payment.Application.Consumers;

public class GamePurchaseValidatedConsumer : IConsumer<GamePurchaseValidatedEvent>
{
    private readonly IPaymentRepository _payments;

    public GamePurchaseValidatedConsumer(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task Consume(ConsumeContext<GamePurchaseValidatedEvent> context)
    {
        // CorrelationId == PaymentId (simplificação para o trabalho)
        var payment = await _payments.GetByIdAsync(context.Message.CorrelationId, context.CancellationToken);

        if (payment is null)
            return;

        if (payment.Status != Domain.Enums.PaymentStatus.Pending)
            return;

        // Atualiza valor com o preço validado pelo serviço de games
        payment.UpdateAmount(context.Message.Price);

        await _payments.UpdateAsync(payment, context.CancellationToken);
    }
}
