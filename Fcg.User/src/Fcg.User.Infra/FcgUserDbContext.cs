using Fcg.User.Infra.Tables;
using Microsoft.EntityFrameworkCore;

namespace Fcg.User.Infra
{
    public class FcgUserDbContext : DbContext
    {
        public FcgUserDbContext(DbContextOptions<FcgUserDbContext> options) : base(options) { }

        public virtual DbSet<Tables.User> Users { get; set; }
        public virtual DbSet<UserGame> UserGames { get; set; }
    }
}
