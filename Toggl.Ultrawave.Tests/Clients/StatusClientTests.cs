using System;
using System.Net;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Xunit;
using static System.Net.HttpStatusCode;

namespace Toggl.Ultrawave.Tests.Clients
{

    public class StatusClientTests
    {
        public class TheGetMethod
        {
            private readonly StatusClient statusClient;
            private readonly IApiClient apiClient = Substitute.For<IApiClient>();

            public TheGetMethod()
            {
                statusClient = new StatusClient(apiClient);
            }

            [Fact]
            public void NeverThrows()
            {
                #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(async x => throw new WebException());
                #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

                statusClient.Get().Wait().Should().BeFalse();
            }

            [Fact]
            public void ReturnsASingleValueForWorkingApiCalls()
            {
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(x => new Response("OK", true, "text/plain", OK));

                statusClient.Get()
                            .Do(Console.WriteLine)
                            .SingleAsync()
                            .Wait()
                            .Should().BeTrue();
            }

            [Fact]
            public void ReturnsASingleValueForFailingApiCalls()
            {
                apiClient
                    .Send(Arg.Any<IRequest>())
                    .Returns(x => new Response("PANIC", false, "text/plain", InternalServerError));

                statusClient.Get()
                            .SingleAsync()
                            .Wait()
                            .Should().BeFalse();
            }
        }
    }
}
