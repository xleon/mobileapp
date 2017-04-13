using System;

namespace Toggl.Ultrawave.Helpers
{
    internal static class ApiUrls
    {
        private const string productionBaseUrl = "https://api.toggl.com/v9/";
        private const string stagingBaseUrl = "https://api.toggl.space/v9/";

        public static Uri ForEnvironment(ApiEnvironment environment)
        {
            switch (environment)
            {
                case ApiEnvironment.Staging:
                    return new Uri(stagingBaseUrl);
                case ApiEnvironment.Production:
                    return new Uri(productionBaseUrl);
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unknown api environment.");
            }
        }
    }
}
