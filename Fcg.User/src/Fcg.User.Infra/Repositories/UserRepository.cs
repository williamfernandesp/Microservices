using Fcg.User.Domain;
using Microsoft.EntityFrameworkCore;

namespace Fcg.User.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FcgUserDbContext _context;

        public UserRepository(FcgUserDbContext context)
        {
            _context = context;
        }

        public async Task BuyGameAsync(Domain.User user)
        {
            foreach (var game in user.Library)
            {
                var entity = await _context.UserGames
                    .FirstOrDefaultAsync(ug => ug.GameId == game.GameId && ug.UserId == user.Id);

                if (entity == null)
                {
                    entity = new Tables.UserGame
                    {
                        Id = game.Id,
                        UserId = user.Id,
                        GameId = game.GameId,
                        DateToPurchase = DateTime.SpecifyKind(game.DateToPurchase.Date, DateTimeKind.Utc)
                    };

                    _context.UserGames.Add(entity);
                }
                else
                {
                    continue;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Domain.User user)
        {
            var entity = new Tables.User
            {
                Id = user.Id
            };

            _context.Users.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<Domain.User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new Domain.User(
                    u.Id,
                    u.Name,
                    u.Wallet,
                    u.Library.Select(ug => new UserGame(
                        ug.Id,
                        ug.UserId,
                        ug.GameId,
                        ug.DateToPurchase))
                    .ToList()
                ))
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(Domain.User user)
        {
            var entity = await _context.Users
                .Include(u => u.Library)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (entity == null)
            {
                entity = new Tables.User
                {
                    Id = user.Id
                };

                _context.Users.Add(entity);
            }

            entity.Name = user.Name;
            entity.Wallet = user.Walllet;

            SaveGames(entity, user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateWalletAsync(Domain.User user)
        {
            var entity = _context.Users.FirstOrDefault(u => u.Id == user.Id);

            if (entity != null)
                entity.Wallet = user.Walllet;

            await _context.SaveChangesAsync();
        }

        private void SaveGames(Tables.User entity, Domain.User user)
        {
            var dbGames = entity.Library;

            var gamesToRemove = dbGames
                .Where(dbGame => !user.Library.Any(domainGame => domainGame.Id == dbGame.Id))
                .ToList();

            foreach (var game in gamesToRemove)
            {
                dbGames.Remove(game);
            }

            foreach (var domainGame in user.Library)
            {
                var dbGame = dbGames.FirstOrDefault(g => g.Id == domainGame.Id);

                if (dbGame == null)
                {
                    var newTableGame = new Tables.UserGame
                    {
                        Id = domainGame.Id,
                        UserId = entity.Id,
                        GameId = domainGame.GameId,
                        DateToPurchase = domainGame.DateToPurchase
                    };

                    dbGames.Add(newTableGame);
                }
                else
                {
                    if (dbGame.GameId != domainGame.GameId)
                        dbGame.GameId = domainGame.GameId;

                    if (dbGame.DateToPurchase != domainGame.DateToPurchase)
                        dbGame.DateToPurchase = domainGame.DateToPurchase;
                }
            }
        }
    }
}
