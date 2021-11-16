using System.Text.Json.Serialization;

namespace Beontime.Application.Authentication.Responses
{

    public sealed class LoginTokenResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        [JsonPropertyName("auth_token")]
        public string AuthToken { get; set; } = "";
        [JsonPropertyName("expires_in")]
        public double ExpiresIn { get; set; }
    }
}
