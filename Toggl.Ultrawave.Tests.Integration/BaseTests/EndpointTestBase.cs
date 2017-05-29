using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class EndpointTestBase
    {
        protected async Task<(ITogglApi togglClient, Ultrawave.User user)> SetupTestUser()
        {
            var credentials = await User.Create();
            var togglApi = TogglApiWith(credentials);
            var user = await togglApi.User.Get();

            return (togglApi, user);
        }

        protected ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(ApiEnvironment.Staging, credentials);
    }
}
