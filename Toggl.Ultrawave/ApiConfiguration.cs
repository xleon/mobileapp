using Toggl.Multivac;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave
{
    public sealed class ApiConfiguration
    {
        public UserAgent UserAgent { get; }
        public Credentials Credentials { get; }
        public ApiEnvironment Environment { get; }

        public ApiConfiguration(ApiEnvironment apiEnvironment, Credentials credentials, UserAgent userAgent)
        {
            Ensure.ArgumentIsNotNull(userAgent, nameof(userAgent));
            Ensure.ArgumentIsNotNull(credentials, nameof(credentials));

            UserAgent = userAgent;
            Credentials = credentials;
            Environment = apiEnvironment;
        }
    }
}
