using Fcg.User.Proxy.Auth.Client;
using Fcg.User.Proxy.Auth.Client.Interface;
using Fcg.User.Proxy.Auth.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Auth
{
    public static class InfraProxyAuthServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraProxyAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientAuth, ClientAuth>();

            services.AddHttpClient<IClientAuth, ClientAuth>();

            services.Configure<AuthConfiguration>(configuration.GetSection("AuthConfiguration"));
            services.TryAddSingleton<IValidateOptions<AuthConfiguration>, AuthConfigurationValidation>();

            return services;
        }
    }
}
