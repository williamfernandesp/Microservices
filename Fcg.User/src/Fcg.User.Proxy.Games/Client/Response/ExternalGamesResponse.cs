using System.Text.Json.Serialization;

namespace Fcg.User.Proxy.Games.Client.Response
{
    public class ExternalGamesResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("genre")]
        public int Genre { get; set; }
    }
}
