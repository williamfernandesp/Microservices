using Fcg.User.Common;
using Fcg.User.Proxy.Payment.Client.Interfaces;
using Fcg.User.Proxy.Payment.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Fcg.User.Proxy.Payment.Client
{
    public class ClientPayment : IClientPayment
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentConfiguration _paymentConfiguration;

        public ClientPayment(HttpClient httpClient, IOptions<PaymentConfiguration> paymentConfiguration)
        {
            _httpClient = httpClient;
            _paymentConfiguration = paymentConfiguration.Value;
        }

        public async Task<Response> ProcessPaymentAsync(Guid userId, Guid gameId)
        {
            var response = new Response();
            var url = $"{_paymentConfiguration.Url}/payments/purchase-game";

            try
            {
                var payload = new { userId, gameId };

                using var httpResponse = await _httpClient.PostAsJsonAsync(url, payload);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    response.AddError($"Payment API Error: {httpResponse.StatusCode} - {errorContent}");
                    return response;
                }

                return response;
            }
            catch (OperationCanceledException)
            {
                response.AddError("The payment request timed out.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddError($"Communication failure: {ex.Message}");
                return response;
            }
        }
    }
}