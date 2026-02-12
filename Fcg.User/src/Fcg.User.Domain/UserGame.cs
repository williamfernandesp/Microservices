namespace Fcg.User.Domain
{
    public class UserGame
    {
        public Guid Id { get; set; }
        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public DateTimeOffset DateToPurchase { get; private set; }

        public UserGame(Guid gameId, DateTimeOffset dateToPurchase)
        {
            Id = Guid.NewGuid();
            GameId = gameId;
            DateToPurchase = dateToPurchase;
        }

        public UserGame(Guid id, Guid userId, Guid gameId, DateTimeOffset dateToPurchase)
        {
            Id = id;
            UserId = userId;
            GameId = gameId;
            DateToPurchase = dateToPurchase;
        }
    }
}
