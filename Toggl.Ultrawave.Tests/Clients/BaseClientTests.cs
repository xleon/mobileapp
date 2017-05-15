﻿using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Ultrawave.Helpers;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Xunit;
using static System.Net.HttpStatusCode;

namespace Toggl.Ultrawave.Tests.Clients
{
    public class BaseClientTests
    {
        public class TheGetAuthHeaderMethod
        {
            [Fact]
            public async Task CreatesRequestWithAppropriateHeaders()
            {
                var apiClient = Substitute.For<IApiClient>();
                var serializer = Substitute.For<IJsonSerializer>();
                var endpoint = Endpoint.Get(ApiUrls.ForEnvironment(ApiEnvironment.Staging), "");
                
                apiClient.Send(Arg.Any<Request>()).Returns(x => new Response("It lives", true, "text/plain", OK));

                var credentials = Credentials.WithPassword("susancalvin@psychohistorian.museum", "theirobotmoviesucked123");
                const string expectedHeader = "c3VzYW5jYWx2aW5AcHN5Y2hvaGlzdG9yaWFuLm11c2V1bTp0aGVpcm9ib3Rtb3ZpZXN1Y2tlZDEyMw==";

                var client = new TestClient(endpoint, apiClient, serializer, credentials);

                client.Get().Wait();

                await apiClient.Received().Send(Arg.Is<Request>(request => verifyAuthHeader(request, expectedHeader)));
            }

            private static bool verifyAuthHeader(Request request, string expectedHeader)
            {
                var authHeader = request.Headers.ToList().Single();

                if (authHeader.Type != HttpHeader.HeaderType.Auth) return false;
                if (authHeader.Value != expectedHeader) return false;

                return true;
            }
        }

        public class TheCreateObservableMethod
        {
            [Fact]
            public void CreatesAnObservableThatReturnsASingleValue()
            {
                var apiClient = Substitute.For<IApiClient>();
                var serializer = Substitute.For<IJsonSerializer>();
                var credentials = Credentials.WithPassword("", "");

                apiClient.Send(Arg.Any<Request>()).Returns(x => new Response("It lives", true, "text/plain", OK));

                var endpoint = Endpoint.Get(ApiUrls.ForEnvironment(ApiEnvironment.Staging), "");
                var client = new TestClient(endpoint, apiClient, serializer, credentials);

                var observable = client.TestCreateObservable<string>(endpoint, Enumerable.Empty<HttpHeader>(), "");
                observable.SingleAsync().Wait();
            }
        }
    }
}
