using Fcg.User.Domain;
using Fcg.User.Domain.Queries;
using Fcg.User.Infra.Queries;
using Fcg.User.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.User.Infra
{
    public static class InfraServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FcgUserDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserQuery, UserQuery>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
