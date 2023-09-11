using System.Net.Http.Headers;

namespace GameServer.Configuration
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

        public static HttpClient HttpClientForMatchServer => httpClientForMatchServer.Value;

        private HttpClientConfig()
        {

        }
    }
}
