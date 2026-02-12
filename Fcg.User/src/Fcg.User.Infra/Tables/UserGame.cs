namespace Fcg.User.Infra.Tables
{
    public class UserGame
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid GameId { get; set; }
        public DateTimeOffset DateToPurchase { get; set; }
    }
}
