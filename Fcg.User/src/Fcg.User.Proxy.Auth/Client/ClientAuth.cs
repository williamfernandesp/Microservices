using Fcg.User.Common;
using Fcg.User.Proxy.Auth.Client.Interface;
using Fcg.User.Proxy.Auth.Client.Responses;
using Fcg.User.Proxy.Auth.Configurations;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Fcg.User.Proxy.Auth.Client
{
    public class ClientAuth : IClientAuth
    {
        private readonly AuthConfiguration _authConfiguration;
        private readonly HttpClient _httpClient;

        public ClientAuth(IOptions<AuthConfiguration> authConfiguration, HttpClient httpClient)
        {
            _authConfiguration = authConfiguration.Value;
            _httpClient = httpClient;
        }

        public async Task<Response<AuthUserResponse>> DeleteAuthUserAsync(Guid id)
        {
            var response = new Response<AuthUserResponse?>();

            var url = $"{_authConfiguration.Url}/auth/users/{id}";

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url);

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var status = (int)httpResponseMessage.StatusCode;
                    var reason = httpResponseMessage.ReasonPhrase ?? "Sem motivo informado";

                    response.AddError($"Erro ao enviar dados ({status} {reason}).");
                    return response;
                }

                var json = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(json))
                    response.Result = JsonSerializer.Deserialize<AuthUserResponse>(json);
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao enviar dados ({ex.Message})");
            }

            return response;
        }

        public async Task<Response<GetUserAuthResponse>> GetAuthUserAsync(Guid id)
        {
            var response = new Response<GetUserAuthResponse?>();

            var url = $"{_authConfiguration.Url}/auth/users/{id}/email";

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var status = (int)httpResponseMessage.StatusCode;
                    var reason = httpResponseMessage.ReasonPhrase ?? "Sem motivo informado";

                    response.AddError($"Erro ao enviar dados ({status} {reason}).");
                    return response;
                }

                var json = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    response.Result = JsonSerializer.Deserialize<GetUserAuthResponse>(json, options);
                }
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao enviar dados ({ex.Message})");
            }

            return response;
        }

        public async Task<Response<AuthUserResponse>> UpdateAuthUserAsync(Guid id, string email)
        {
            var response = new Response<AuthUserResponse?>();

            var url = $"{_authConfiguration.Url}/auth/users/{id}/email";

            var payload = new
            {
                email
            };

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(payload),
                        Encoding.UTF8,
                        "application/json"
                    )
                };

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var status = (int)httpResponseMessage.StatusCode;
                    var reason = httpResponseMessage.ReasonPhrase ?? "Sem motivo informado";
                    var body = await httpResponseMessage.Content.ReadAsStringAsync();

                    response.AddError($"Erro ao enviar dados ({status} {reason}). Detalhes: {body}");
                    return response;
                }

                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                response.Result = JsonSerializer.Deserialize<AuthUserResponse>(json);
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao enviar dados ({ex.Message})");
            }

            return response;
        }
    }
}
