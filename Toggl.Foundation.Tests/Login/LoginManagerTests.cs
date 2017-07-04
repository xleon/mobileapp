﻿using System;
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
using static Toggl.Foundation.Login.LoginManager;
using User = Toggl.Ultrawave.Models.User;

namespace Toggl.Foundation.Tests.Login
{
    public class LoginManagerTests
    {
        public class Constructor
        {
            private static readonly IApiFactory ApiFactory = Substitute.For<IApiFactory>();
            private static readonly DatabaseFactory DatabaseFactory = () => Substitute.For<ITogglDatabase>();

            [Theory]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useApiFactory, bool useDatabaseFactory)
            {
                var apiFactory = useApiFactory ? ApiFactory : null;
                var databaseFactory = useDatabaseFactory ? DatabaseFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new LoginManager(apiFactory, databaseFactory);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheLoginMethod
        {
            private const string Password = "theirobotmoviesucked123";
            private static readonly Email Email = "susancalvin@psychohistorian.museum".ToEmail();

            private readonly LoginManager loginManager;
            private readonly User user = new User { Id = 10 };
            private readonly ITogglApi api = Substitute.For<ITogglApi>();
            private readonly IApiFactory apiFactory = Substitute.For<IApiFactory>();
            private readonly ITogglDatabase database = Substitute.For<ITogglDatabase>();

            public TheLoginMethod()
            {
                loginManager = new LoginManager(apiFactory, () => database);

                api.User.Get().Returns(Observable.Return(user));
                apiFactory.CreateApiWith(Arg.Any<Credentials>()).Returns(api);
                database.Clear().Returns(Observable.Return(Unit.Default));
            }

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
                    () => loginManager.Login(actualEmail, password).Wait();

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentException>();
            }

            [Fact]
            public async Task EmptiesTheDatabaseBeforeTryingToLogin()
            {
                await loginManager.Login(Email, Password);

                Received.InOrder(async () =>
                {
                    await database.Clear();
                    await api.User.Get();
                });
            }

            [Fact]
            public async Task CallsTheGetMethodOfTheUserApi()
            {
                await loginManager.Login(Email, Password);

                await api.User.Received().Get();
            }

            [Fact]
            public async Task ShouldPersistTheUserToTheDatabase()
            {
                await loginManager.Login(Email, Password);

                await database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == user.Id));
            }

            [Fact]
            public async Task TheUserToBePersistedShouldHaveIsDirtySetToFalse()
            {
                await loginManager.Login(Email, Password);

                await database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.IsDirty == false));
            }

            [Fact]
            public async Task ShouldAlwaysReturnASingleResult()
            {
                await loginManager
                        .Login(Email, Password)
                        .SingleAsync();
            }
        }
    }
}
