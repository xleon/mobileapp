using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using Xunit;
using Toggl.Ultrawave.Tests.Integration.BaseTests;

namespace Toggl.Ultrawave.Tests.Integration
{
    public sealed class StatusApiTests
    {
        public sealed class TheGetMethod : EndpointTestBase
        {
            [Fact, LogTestInfo]
            public async Task ShouldSucceedWithoutCredentials()
            {
                var togglClient = TogglApiWith(Credentials.None);

                var status = await togglClient.Status.Get();

                status.Should().Be(true);
            }
        }
    }
}
