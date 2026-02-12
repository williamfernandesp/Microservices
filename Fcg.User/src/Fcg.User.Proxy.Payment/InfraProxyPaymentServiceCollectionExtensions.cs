using Fcg.User.Proxy.Payment.Client;
using Fcg.User.Proxy.Payment.Client.Interfaces;
using Fcg.User.Proxy.Payment.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Payment
{
    public static class InfraProxyPaymentServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraProxyPayment(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientPayment, ClientPayment>();

            services.AddHttpClient<IClientPayment, ClientPayment>();

            services.Configure<PaymentConfiguration>(configuration.GetSection("PaymentConfiguration"));
            services.TryAddSingleton<IValidateOptions<PaymentConfiguration>, PaymentConfigurationValidation>();

            return services;
        }
    }
}
