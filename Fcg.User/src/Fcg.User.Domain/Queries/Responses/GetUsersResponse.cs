namespace Fcg.User.Domain.Queries.Responses
{
    public class GetUsersResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public decimal Wallet { get; set; }
    }
}
