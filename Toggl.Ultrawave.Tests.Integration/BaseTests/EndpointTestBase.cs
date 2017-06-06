using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class EndpointTestBase
    {
        private static readonly UserAgent userAgent = new UserAgent("MobileIntegrationTests", "d7be465f099633a0dfaf741ba10d54dafb4a5bf7");

        protected async Task<(ITogglApi togglClient, Models.User user)> SetupTestUser()
        {
            var credentials = await User.Create();
            var togglApi = TogglApiWith(credentials);
            var user = await togglApi.User.Get();

            return (togglApi, user);
        }

        protected ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(configurationFor(credentials));

        private ApiConfiguration configurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Staging, credentials, userAgent);
    }
}
