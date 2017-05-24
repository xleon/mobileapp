using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Foundation.Tests.DataSources
{
    public class UserDataSourceTests
    {
        public class TheLoginMethod
        {
            private readonly User user = new User { Id = 10 } ;

            private readonly IUserClient userClient = Substitute.For<IUserClient>();
            private readonly ISingleObjectStorage<IDatabaseUser> storage =
                Substitute.For<ISingleObjectStorage<IDatabaseUser>>();

            private readonly UserDataSource userSource;

            public TheLoginMethod()
            {
                userSource = new UserDataSource(storage, userClient);

                userClient.Get(Arg.Any<Credentials>()).Returns(Observable.Return(user));
            }

            [Fact]
            public async Task ShouldCreateCredentialsInsteadOfUsingTheOnesTheUserClientHas()
            {
                await userSource.Login("susancalvin@psychohistorian.museum".ToEmail(), "theirobotmoviesucked123");
                await userClient.Received().Get(Arg.Any<Credentials>());
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
