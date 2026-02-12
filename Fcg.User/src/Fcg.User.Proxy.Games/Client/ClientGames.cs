using Fcg.User.Common;
using Fcg.User.Proxy.Games.Client.Interface;
using Fcg.User.Proxy.Games.Client.Response;
using Fcg.User.Proxy.Games.Configurations;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Fcg.User.Proxy.Games.Client
{
    public class ClientGames : IClientGames
    {
        private readonly HttpClient _httpClient;
        private readonly GamesConfiguration _gamesConfiguration;

        public ClientGames(HttpClient httpClient, IOptions<GamesConfiguration> gamesConfiguration)
        {
            _httpClient = httpClient;
            _gamesConfiguration = gamesConfiguration.Value;
        }

        public async Task<Response<ExternalGamesResponse>> GetGameAsync(Guid id)
        {
            var response = new Response<ExternalGamesResponse>();

            var url = $"{_gamesConfiguration.Url}/api/games/{id}";

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
                    response.Result = JsonSerializer.Deserialize<ExternalGamesResponse>(json);
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao receber dados ({ex.Message})");
            }

            return response;
        }

        public async Task<Response<IEnumerable<ExternalGamesResponse>>> GetGamesAsync(IEnumerable<Guid> ids)
        {
            var response = new Response<IEnumerable<ExternalGamesResponse>>();

            var queryString = string.Join("&", ids.Select(id => $"gameIds={id}"));
            var url = $"{_gamesConfiguration.Url}/api/games/ids?{queryString}";

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
                    response.Result = JsonSerializer.Deserialize<IEnumerable<ExternalGamesResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                response.AddError($"Erro ao receber dados ({ex.Message})");
            }

            return response;
        }
    }
}
