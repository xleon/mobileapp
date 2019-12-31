using System.Net.Http;
using Toggl.Networking.Network;

namespace Toggl.Networking.Tests.Integration.Helper
{
    internal static class TogglApiFactory
    {
        public static ITogglApi TogglApiWith(Credentials credentials)
        {
            var apiConfiguration = configurationFor(credentials);
            var httpClient = new HttpClient();
            var apiClient = new ApiClient(httpClient, apiConfiguration.UserAgent);
            var retryingApiClient = new RetryingApiClient(apiClient);

            return new TogglApi(apiConfiguration, retryingApiClient);
        }

        private static ApiConfiguration configurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Staging, credentials, Configuration.UserAgent);
    }
}
