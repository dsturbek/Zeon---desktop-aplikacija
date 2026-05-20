using ApiServiceLayer.DTOs;
using System.Threading.Tasks;

namespace ApiServiceLayer.Services
{
    public class AuthApiService
    {
        private readonly ApiClient _apiClient;

        public AuthApiService()
        {
            _apiClient = ApiClient.Instance;
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password,
                Role = "trainer"
            };

            return await _apiClient.PostAsync<LoginRequest, LoginResponse>("/api/auth/login", request);
        }

        public async Task<LogoutResponse> LogoutAsync()
        {
            return await _apiClient.PostAsync<LogoutResponse>("/api/auth/logout");
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            return await _apiClient.GetAsync<UserDto>("/api/auth/me");
        }

        public bool IsAuthenticated()
        {
            return _apiClient.HasSessionCookie();
        }

        public void ClearSession()
        {
            _apiClient.ClearCookies();
        }
    }

    public class LogoutResponse
    {
        public string Message { get; set; }
    }
}
