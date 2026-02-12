using System.Text.Json.Serialization;

namespace Fcg.User.Proxy.Auth.Client.Responses
{
    public class AuthUserResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
    }
}
