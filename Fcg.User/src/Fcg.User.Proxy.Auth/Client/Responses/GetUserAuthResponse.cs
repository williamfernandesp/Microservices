using System.Text.Json.Serialization;

namespace Fcg.User.Proxy.Auth.Client.Responses
{
    public class GetUserAuthResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
    }
}
