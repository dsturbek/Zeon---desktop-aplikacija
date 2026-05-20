using Newtonsoft.Json;

namespace ApiServiceLayer.DTOs
{
    public class LoginResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("user")]
        public UserDto User { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
