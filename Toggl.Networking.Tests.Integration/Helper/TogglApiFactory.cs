using System.Net.Http;
using Toggl.Networking.Network;
using static System.Net.DecompressionMethods;

namespace Toggl.Networking.Tests.Integration.Helper
{
    internal static class TogglApiFactory
    {
        public static HttpClient CreateHttpClientForIntegrationTests()
        {
            var httpHandler = new HttpClientHandler { AutomaticDecompression = GZip | Deflate };
            return new HttpClient(httpHandler, true);
        }

        public static ITogglApi CreateTogglApiWith(Credentials credentials)
        {
            var apiConfiguration = configurationFor(credentials);
            var apiClient = new ApiClient(CreateHttpClientForIntegrationTests(), apiConfiguration.UserAgent);
            var retryingApiClient = new RetryingApiClient(apiClient);

            return new TogglApi(apiConfiguration, retryingApiClient);
        }

        private static ApiConfiguration configurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Staging, credentials, Configuration.UserAgent);
    }
}
