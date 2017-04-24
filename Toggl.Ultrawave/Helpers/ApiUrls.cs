using System;

namespace Toggl.Ultrawave.Helpers
{
    internal static class ApiUrls
    {
        private const string productionBaseUrl = "https://toggl.com/api/v9/";
        private const string stagingBaseUrl = "https://toggl.space/api/v9/";

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
