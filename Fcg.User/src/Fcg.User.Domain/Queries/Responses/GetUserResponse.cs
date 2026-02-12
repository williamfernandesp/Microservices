namespace Fcg.User.Domain.Queries.Responses
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public decimal Wallet { get; set; }
        public string? Email { get; set; }
        public List<GameResponse> Library { get; set; } = new();
    }
}
