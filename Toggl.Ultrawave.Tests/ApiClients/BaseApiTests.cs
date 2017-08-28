﻿using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Multivac.Extensions;
using Toggl.Ultrawave.Helpers;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Xunit;
using static System.Net.HttpStatusCode;

namespace Toggl.Ultrawave.Tests.ApiClients
{
    public sealed class BaseApiTests
    {
        public sealed class TheGetAuthHeaderMethod
        {
            [Fact]
            public async Task CreatesRequestWithAppropriateHeaders()
            {
                var apiClient = Substitute.For<IApiClient>();
                var serializer = Substitute.For<IJsonSerializer>();
                var endpoint = Endpoint.Get(ApiUrls.ForEnvironment(ApiEnvironment.Staging), "");
                
                apiClient.Send(Arg.Any<Request>()).Returns(x => new Response("It lives", true, "text/plain", OK));

                var credentials = Credentials.WithPassword("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123");
                const string expectedHeader = "c3VzYW5jYWx2aW5AcHN5Y2hvaGlzdG9yaWFuLm11c2V1bTp0aGVpcm9ib3Rtb3ZpZXN1Y2tlZDEyMw==";

                var testApi = new TestApi(endpoint, apiClient, serializer, credentials);

                await testApi.Get();
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

        public sealed class TheCreateObservableMethod
        {
            [Fact]
            public async Task CreatesAnObservableThatReturnsASingleValue()
            {
                var apiClient = Substitute.For<IApiClient>();
                var serializer = Substitute.For<IJsonSerializer>();
                var credentials = Credentials.WithPassword("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123");

                apiClient.Send(Arg.Any<Request>()).Returns(x => new Response("It lives", true, "text/plain", OK));

                var endpoint = Endpoint.Get(ApiUrls.ForEnvironment(ApiEnvironment.Staging), "");
                var testApi = new TestApi(endpoint, apiClient, serializer, credentials);

                var observable = testApi.TestCreateObservable<string>(endpoint, Enumerable.Empty<HttpHeader>(), "");
                await observable.SingleAsync();
            }
        }
    }
}
