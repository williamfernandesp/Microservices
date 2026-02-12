namespace Fcg.User.Domain
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task SaveAsync(User user);
        Task DeleteAsync(User user);
        Task UpdateWalletAsync(User user);
        Task BuyGameAsync(User user);
    }
}
