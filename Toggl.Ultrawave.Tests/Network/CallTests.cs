using System;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using System.Net;

namespace Toggl.Ultrawave.Tests.Network
{
    public class CallTests
    {
        public class TheExecuteMethod
        {
            private readonly IRequest request = Substitute.For<IRequest>();
            private readonly IApiClient apiClient = Substitute.For<IApiClient>();
            private readonly IJsonSerializer serializer = Substitute.For<IJsonSerializer>();

            private class Foo
            {
                public string Bar { get; set; }
            }

            [Fact]
            public void CanOnlyBeCalledOnce()
            {
                var call = new Call<string>(request, apiClient, serializer);

                Func<Task> callingTheSameCallTwice = async () =>
                {
                    await call.Execute();
                    await call.Execute();
                };

                callingTheSameCallTwice
                    .ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public async Task ReturnsDeserializedDataIfTheApiCallWorks()
            {
                const string json = "{\"bar\":\"Baz\"}";
                var deserializedJson = new Foo { Bar = "Baz" };
                var response = new Response(json, true, "text/plain", HttpStatusCode.OK);

                apiClient.Send(request).Returns(response);
                serializer.Deserialize<Foo>(response.RawData).Returns(deserializedJson);

                var call = new Call<Foo>(request, apiClient, serializer);
                var result = await call.Execute();

                result.Data.Left.Should().Be(deserializedJson);
            }

            [Fact]
            public async Task ReturnsAnErrorMessageIfTheApiCallDoesNotWork()
            {
                const string error = "Bad request";
                var response = new Response(error, false, "text/plain", HttpStatusCode.BadRequest);

                apiClient.Send(request).Returns(response);

                var call = new Call<Foo>(request, apiClient, serializer);
                var result = await call.Execute();

                result.Data.Right.ToString().Should().Be(error);
            }
        }
    }
}
