using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Ultrawave.ApiClients;
using Toggl.Ultrawave.Helpers;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Xunit;
using static System.Net.HttpStatusCode;

namespace Toggl.Ultrawave.Tests.ApiClients
{
    public class UserApiTests
    {
        public class TheGetMethod
        {
            [Fact]
            public async Task UsesTheCredentialsThatYouProvide()
            {

                var credentials = Credentials.WithApiToken("123");
                var apiClient = Substitute.For<IApiClient>();
                var serializer = Substitute.For<IJsonSerializer>();
                var endpoints = new UserEndpoints(ApiUrls.ForEnvironment(ApiEnvironment.Staging));

                apiClient.Send(Arg.Any<Request>()).Returns(x => new Response("It lives", true, "text/plain", OK));
                var userApi = new UserApi(endpoints, apiClient, serializer, Credentials.None);

                await userApi.Get(credentials);

                await apiClient.Received()
                               .Send(Arg.Is<Request>(
                                   request => request.Headers.Single().Type != HttpHeader.HeaderType.None)
                               );
            }
        }
    }
}
