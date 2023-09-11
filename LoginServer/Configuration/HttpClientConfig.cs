using System.Net.Http.Headers;

namespace LoginServer.Configuration
{
    public class HttpClientConfig
    {
        private static readonly Lazy<HttpClient> httpClientForMatchServer = new Lazy<HttpClient>(() =>
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ServerConfig.MatchServerPrivateAddress);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ServerSessionId", ServerConfig.ServerSessionId);
            return httpClient;
        });

        private static readonly Lazy<HttpClient> httpClientForGameServer = new Lazy<HttpClient>(() =>
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ServerConfig.GameServerPrivateAddress);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ServerSessionId", ServerConfig.ServerSessionId);
            return httpClient;
        });

        public static HttpClient HttpClientForMatchServer => httpClientForMatchServer.Value;
        public static HttpClient HttpClientForGameServer => httpClientForGameServer.Value;

        private HttpClientConfig()
        {

        }
    }
}
