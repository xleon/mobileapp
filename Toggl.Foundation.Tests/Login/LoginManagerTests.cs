using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Login;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using Xunit;
using User = Toggl.Ultrawave.Models.User;
using FoundationUser = Toggl.Foundation.Models.User;
using Toggl.Foundation.Tests.Generators;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.Tests.Login
{
    public class LoginManagerTests
    {
        public abstract class LoginManagerTest
        {
            protected const string Password = "theirobotmoviesucked123";
            protected static readonly Email Email = "susancalvin@psychohistorian.museum".ToEmail();
            
            protected readonly IUser User = new User { Id = 10, ApiToken = "ABCDEFG" };
            protected readonly ITogglApi Api = Substitute.For<ITogglApi>();
            protected readonly IApiFactory ApiFactory = Substitute.For<IApiFactory>();
            protected readonly ITogglDatabase Database = Substitute.For<ITogglDatabase>();

            protected readonly ILoginManager LoginManager;

            protected LoginManagerTest()
            {
                LoginManager = new LoginManager(ApiFactory, Database);

                Api.User.Get().Returns(Observable.Return(User));
                ApiFactory.CreateApiWith(Arg.Any<Credentials>()).Returns(Api);
                Database.Clear().Returns(Observable.Return(Unit.Default));
            }
        }

        public class Constructor : LoginManagerTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useApiFactory, bool useDatabase)
            {
                var database = useDatabase ? Database : null;
                var apiFactory = useApiFactory ? ApiFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new LoginManager(apiFactory, database);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheLoginMethod : LoginManagerTest
        {
            [Theory]
            [InlineData("susancalvin@psychohistorian.museum", null)]
            [InlineData("susancalvin@psychohistorian.museum", "")]
            [InlineData("susancalvin@psychohistorian.museum", " ")]
            [InlineData("susancalvin@", null)]
            [InlineData("susancalvin@", "")]
            [InlineData("susancalvin@", " ")]
            [InlineData("susancalvin@", "123456")]
            [InlineData("", null)]
            [InlineData("", "")]
            [InlineData("", " ")]
            [InlineData("", "123456")]
            [InlineData(null, null)]
            [InlineData(null, "")]
            [InlineData(null, " ")]
            [InlineData(null, "123456")]
            public void ThrowsIfYouPassInvalidParameters(string email, string password)
            {
                var actualEmail = Email.FromString(email);

                Action tryingToConstructWithEmptyParameters =
                    () => LoginManager.Login(actualEmail, password).Wait();

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentException>();
            }

            [Fact]
            public async Task EmptiesTheDatabaseBeforeTryingToLogin()
            {
                await LoginManager.Login(Email, Password);

                Received.InOrder(async () =>
                {
                    await Database.Clear();
                    await Api.User.Get();
                });
            }

            [Fact]
            public async Task CallsTheGetMethodOfTheUserApi()
            {
                await LoginManager.Login(Email, Password);

                await Api.User.Received().Get();
            }

            [Fact]
            public async Task ShouldPersistTheUserToTheDatabase()
            {
                await LoginManager.Login(Email, Password);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == User.Id));
            }

            [Fact]
            public async Task TheUserToBePersistedShouldHaveIsDirtySetToFalse()
            {
                await LoginManager.Login(Email, Password);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.IsDirty == false));
            }

            [Fact]
            public async Task ShouldAlwaysReturnASingleResult()
            {
                await LoginManager
                        .Login(Email, Password)
                        .SingleAsync();
            }
        }

        public class TheGetDataSourceIfLoggedInInMethod : LoginManagerTest
        {
            [Fact]
            public void ReturnsNullIfTheDatabaseHasNoUsers()
            {
                var observable = Observable.Throw<IDatabaseUser>(new InvalidOperationException());
                Database.User.Single().Returns(observable);

                var result = LoginManager.GetDataSourceIfLoggedIn();

                result.Should().BeNull();
            }

            [Fact]
            public void ReturnsADataSourceIfTheUserExistsInTheDatabase()
            {
                var observable = Observable.Return<IDatabaseUser>(FoundationUser.Clean(User));
                Database.User.Single().Returns(observable);

                var result = LoginManager.GetDataSourceIfLoggedIn();

                result.Should().NotBeNull();
            }
        }
    }
}
