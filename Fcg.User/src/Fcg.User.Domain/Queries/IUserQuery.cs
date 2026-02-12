using Fcg.User.Common;
using Fcg.User.Domain.Queries.Responses;

namespace Fcg.User.Domain.Queries
{
    public interface IUserQuery
    {
        Task<GetUserResponse?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetGamesByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResponse<GetUsersResponse>> GetUsersAsync(int skip, int take);
    }
}
