using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiServiceLayer.Services
{
    public class ApiClient
    {
        private static ApiClient _instance;
        private static readonly object _lock = new object();

        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

        public const string BaseUrl = "https://zeon-web-server.onrender.com";

        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApiClient();
                        }
                    }
                }
                return _instance;
            }
        }

        private ApiClient()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<T> PostAsync<T>(string endpoint)
        {
            var response = await _httpClient.PostAsync(endpoint, null);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public void ClearCookies()
        {
            var cookies = _cookieContainer.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        public bool HasSessionCookie()
        {
            var cookies = _cookieContainer.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "connect.sid" && !cookie.Expired)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseContent { get; }

        public ApiException(HttpStatusCode statusCode, string responseContent)
            : base($"API Error: {statusCode} - {responseContent}")
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}
