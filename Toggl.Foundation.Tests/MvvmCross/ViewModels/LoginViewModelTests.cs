using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Models;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class LoginViewModelTests
    {
        public abstract class LoginViewModelTest : BaseViewModelTests<LoginViewModel>
        {
            protected const string ValidEmail = "susancalvin@psychohistorian.museum";
            protected const string InvalidEmail = "foo@";

            protected TestScheduler TestScheduler { get; } = new TestScheduler();
            protected IUser User { get; } = new User { Id = 10, ApiToken = "1337" };

            protected ILoginManager LoginManager { get; } = Substitute.For<ILoginManager>();

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                Ioc.RegisterSingleton(LoginManager);
            }

            protected override LoginViewModel CreateViewModel()
                => new LoginViewModel(LoginManager);
        }

        public class TheConstructor : LoginViewModelTests
        {
            private static readonly IApiFactory ApiFactory = Substitute.For<IApiFactory>();
            private static readonly ITogglDataSource DataSource = Substitute.For<ITogglDataSource>();
            
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new LoginViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
