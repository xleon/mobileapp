using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using Xunit;
using Toggl.Ultrawave.Tests.Integration.BaseTests;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class StatusClientTests
    {
        public class TheGetMethod : EndpointTestBase
        {
            [Fact]
            public async Task ShouldSucceedWithoutCredentials()
            {
                var togglClient = TogglClientWith(Credentials.None);

                var status = await togglClient.Status.Get();

                status.Should().Be(true);
            }
        }
    }
}
