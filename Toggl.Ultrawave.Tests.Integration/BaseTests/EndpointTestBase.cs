using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class EndpointTestBase
    {
        protected async Task<(ITogglClient togglClient, Ultrawave.User user)> SetupTestUser()
        {
            var credentials = await User.Create();
            var togglClient = TogglClientWith(credentials);
            var user = await togglClient.User.Get();

            return (togglClient, user);
        }

        protected ITogglClient TogglClientWith(Credentials credentials)
            => new TogglClient(ApiEnvironment.Staging, credentials);
    }
}
