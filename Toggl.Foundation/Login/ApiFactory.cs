using Toggl.Multivac;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.Login
{
    public sealed class ApiFactory : IApiFactory
    {
        public UserAgent UserAgent { get; }

        public ApiEnvironment Environment { get; }

        public ApiFactory(ApiEnvironment apiEnvironment, UserAgent userAgent)
        {
            Ensure.Argument.IsNotNull(userAgent, nameof(userAgent));

            UserAgent = userAgent;
            Environment = apiEnvironment;
        }

        public ITogglApi CreateApiWith(Credentials credentials)
            => new TogglApi(new ApiConfiguration(Environment, credentials, UserAgent));
    }
}
