using Fcg.Contracts.Payments;
using Fcg.User.Infra;
using Fcg.User.Infra.Tables;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Fcg.User.Web.Consumers;

public class PaymentApprovedConsumer : IConsumer<PaymentApprovedEvent>
{
    private readonly FcgUserDbContext _db;

    public PaymentApprovedConsumer(FcgUserDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PaymentApprovedEvent> context)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == context.Message.UserId, context.CancellationToken);

        if (user is null)
            return;

        // Crédito de carteira (quando GameId é null)
        if (context.Message.GameId is null)
        {
            user.Wallet += context.Message.Amount;
            await _db.SaveChangesAsync(context.CancellationToken);
            return;
        }

        // Compra de jogo (GameId != null) -> adiciona na biblioteca
        var gameId = context.Message.GameId.Value;

        var alreadyOwned = await _db.UserGames
            .AnyAsync(ug => ug.UserId == context.Message.UserId && ug.GameId == gameId, context.CancellationToken);

        if (alreadyOwned)
            return;

        _db.UserGames.Add(new UserGame
        {
            Id = Guid.NewGuid(),
            UserId = context.Message.UserId,
            GameId = gameId,
            DateToPurchase = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
