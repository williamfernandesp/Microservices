using Fcg.User.Proxy.Games.Client;
using Fcg.User.Proxy.Games.Client.Interface;
using Fcg.User.Proxy.Games.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Games
{
    public static class InfraProxyGamesServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraProxyGames(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientGames, ClientGames>();

            services.AddHttpClient<IClientGames, ClientGames>();

            services.Configure<GamesConfiguration>(configuration.GetSection("GamesConfiguration"));
            services.TryAddSingleton<IValidateOptions<GamesConfiguration>, GamesConfigurationValidation>();

            return services;
        }
    }
}
