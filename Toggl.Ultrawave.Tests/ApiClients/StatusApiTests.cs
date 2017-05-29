using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Ultrawave.ApiClients;
using Toggl.Ultrawave.Network;
using Xunit;
using static System.Net.HttpStatusCode;

namespace Toggl.Ultrawave.Tests.Clients
{
    public class StatusClientTests
    {
        public class TheGetMethod
        {
            private readonly StatusApi statusApi;
            private readonly IApiClient apiClient = Substitute.For<IApiClient>();

            public TheGetMethod()
            {
                statusApi = new StatusApi(apiClient);
            }

            [Fact]
            public async Task NeverThrows()
            {
                #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(async x => throw new WebException());
                #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

                var status = await statusApi.Get();
                status.Should().BeFalse();
            }

            [Fact]
            public async Task ReturnsASingleValueForWorkingApiCalls()
            {
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(x => new Response("OK", true, "text/plain", OK));

                var status = await statusApi.Get().SingleAsync();
                status.Should().BeTrue();
            }

            [Fact]
            public async Task ReturnsASingleValueForFailingApiCalls()
            {
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(x => new Response("PANIC", false, "text/plain", InternalServerError));

                var status = await statusApi.Get().SingleAsync();
                status.Should().BeFalse();
            }
        }
    }
}
