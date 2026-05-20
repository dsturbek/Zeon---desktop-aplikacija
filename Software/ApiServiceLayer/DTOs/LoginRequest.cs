using Newtonsoft.Json;

namespace ApiServiceLayer.DTOs
{
    public class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } = "trainer";
    }
}
