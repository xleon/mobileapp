using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.ApiClients;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Foundation.Tests.DataSources
{
    public class UserDataSourceTests
    {
        public class TheLoginMethod
        {
            private readonly User user = new User { Id = 10 } ;

            private readonly IUserApi userApi = Substitute.For<IUserApi>();
            private readonly ISingleObjectStorage<IDatabaseUser> storage =
                Substitute.For<ISingleObjectStorage<IDatabaseUser>>();

            private readonly UserDataSource userSource;

            public TheLoginMethod()
            {
                userSource = new UserDataSource(storage, userApi);

                userApi.Get(Arg.Any<Credentials>()).Returns(Observable.Return(user));
            }

            [Fact]
            public async Task ShouldCreateCredentialsInsteadOfUsingTheOnesTheUserClientHas()
            {
                await userSource.Login("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123");
                await userApi.Received().Get(Arg.Any<Credentials>());
            }

            [Fact]
            public async Task ShouldPersistTheUserToTheDatabase()
            {
                await userSource.Login("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123");
                await storage.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == user.Id));
            }

            [Fact]
            public async Task ShouldAlwaysReturnASingleResult()
            {
                var actualUser = await userSource.Login("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123")
                                                 .SingleAsync();

                actualUser.Should().Be(user);
            }
        }
    }
}
