using Fcg.User.Common;
using Fcg.User.Domain.Queries;
using Fcg.User.Domain.Queries.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fcg.User.Infra.Queries
{
    public class UserQuery : IUserQuery
    {
        private readonly FcgUserDbContext _context;

        public UserQuery(FcgUserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guid>> GetGamesByUserIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await(from u in _context.UserGames
                         where u.UserId == id
                         select u.GameId).ToListAsync(cancellationToken);
        }

        public async Task<GetUserResponse?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await (from u in _context.Users
                              where u.Id == id
                              select new GetUserResponse
                              {
                                  Id = u.Id,
                                  UserName = u.Name,
                                  Wallet = u.Wallet,
                              }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResponse<GetUsersResponse>> GetUsersAsync(int skip, int take)
        {
            var users = await (from u in _context.Users
                               orderby u.Name
                               select new GetUsersResponse
                               {
                                   Id = u.Id,
                                   UserName = u.Name,
                                   Wallet = u.Wallet
                               })
                               .Skip(skip)
                               .Take(take)
                               .ToListAsync();

            var totalUsers = await _context.Users.CountAsync();

            return new PagedResponse<GetUsersResponse>
            (
                users,
                totalUsers,
                skip,
                take
            );
        }
    }
}
