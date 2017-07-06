using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Xunit;
using static Toggl.Foundation.MvvmCross.Parameters.LoginParameter;
using User = Toggl.Ultrawave.Models.User;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class LoginViewModelTests
    {
        public abstract class LoginViewModelTest : BaseViewModelTests<LoginViewModel>
        {
            protected const string ValidEmail = "susancalvin@psychohistorian.museum";
            protected const string InvalidEmail = "foo@";

            protected const string ValidPassword = "123456";
            protected const string InvalidPassword = "";

            protected TestScheduler TestScheduler { get; } = new TestScheduler();
            protected IUser User { get; } = new User { Id = 10, ApiToken = "1337" };

            protected ILoginManager LoginManager { get; } = Substitute.For<ILoginManager>();

            protected override LoginViewModel CreateViewModel()
                => new LoginViewModel(LoginManager, NavigationService);
        }

        public class TheConstructor : LoginViewModelTest
        {
            [Theory]
            [InlineData(false, false)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool userLoginManager, bool userNavigationService)
            {
                var loginManager = userLoginManager ? LoginManager : null;
                var navigationService = userNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new LoginViewModel(loginManager, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheNextIsEnabledProperty
        {
            public class WhenInTheEmailPage : LoginViewModelTest
            {
                [Fact]
                public void ReturnsFalseIfTheEmailIsInvalid()
                {
                    ViewModel.Email = InvalidEmail;

                    ViewModel.NextIsEnabled.Should().BeFalse();
                }

                [Fact]
                public void ReturnsTrueIfTheEmailIsValid()
                {
                    ViewModel.Email = ValidEmail;

                    ViewModel.NextIsEnabled.Should().BeTrue();
                }
            }

            public class WhenInThePasswordPage : LoginViewModelTest
            {
                public WhenInThePasswordPage()
                {
                    ViewModel.Email = ValidEmail;
                    ViewModel.NextCommand.Execute();
                }

                [Fact]
                public void ReturnsFalseWhenThePasswordIsNotValid()
                {
                    ViewModel.Password = InvalidPassword;

                    ViewModel.NextIsEnabled.Should().BeFalse();
                }

                [Fact]
                public void ReturnsTrueIfThePasswordIsValid()
                {
                    ViewModel.Password = ValidPassword;

                    ViewModel.NextIsEnabled.Should().BeTrue();
                }

                [Fact]
                public void ReturnsFalseWheThePasswordIsValidButTheViewIsLoading()
                {
                    var scheduler = new TestScheduler();
                    var never = Observable.Never<ITogglDataSource>();
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>()).Returns(never);
                    ViewModel.Password = ValidPassword;
                    ViewModel.NextCommand.Execute();
  
                    ViewModel.NextCommand.Execute();

                    ViewModel.NextIsEnabled.Should().BeFalse();
                }
            }
        }

        public class TheInitializeMethod : LoginViewModelTest
        {
            [Fact]
            public async Task SetsTheLoginType()
            {
                ViewModel.LoginType = (LoginType)1000;
                var parameter = LoginParameter.Login;

                await ViewModel.Initialize(parameter);

                ViewModel.LoginType.Should().Be(LoginType.Login);
            }

            [Fact]
            public async Task SetsTheTitleToLoginWhenThePassedParameterIsLogin()
            {
                var parameter = LoginParameter.Login;

                await ViewModel.Initialize(parameter);

                ViewModel.Title.Should().Be(Resources.LoginTitle);
            }

            [Fact]
            public async Task SetsTheTitleToSignupWhenThePassedParameterIsLogin()
            {
                var parameter = LoginParameter.SignUp;

                await ViewModel.Initialize(parameter);

                ViewModel.Title.Should().Be(Resources.SignUpTitle);
            }
        }

        public class TheNextCommand
        {
            public class WhenInTheEmailPage : LoginViewModelTest
            {
                [Fact]
                public void DoesNothingWhenTheEmailIsInvalid()
                {
                    ViewModel.Email = InvalidEmail;

                    ViewModel.NextCommand.Execute();

                    ViewModel.CurrentPage.Should().Be(LoginViewModel.EmailPage);
                }

                [Fact]
                public void ShowsThePasswordPageWhenTheEmailIsValid()
                {
                    ViewModel.Email = ValidEmail;

                    ViewModel.NextCommand.Execute();

                    ViewModel.CurrentPage.Should().Be(LoginViewModel.PasswordPage);
                }
            }

            public class WhenInThePasswordPage : LoginViewModelTest
            {
                public WhenInThePasswordPage()
                {
                    ViewModel.Email = ValidEmail;
                    ViewModel.NextCommand.Execute();
                }

                [Fact]
                public void DoesNotAttemptToLoginWhileThePasswordIsValid()
                {
                    ViewModel.Password = InvalidPassword;

                    ViewModel.NextCommand.Execute();

                    LoginManager.DidNotReceive().Login(Arg.Any<Email>(), Arg.Any<string>());
                }

                [Fact]
                public void CallsTheLoginManagerWhenThePasswordIsValid()
                {
                    ViewModel.Password = ValidPassword;

                    ViewModel.NextCommand.Execute();

                    LoginManager.Received().Login(Arg.Any<Email>(), Arg.Any<string>());
                }

                [Fact]
                public void DoesNothingWhenThePageIsCurrentlyLoading()
                {
                    var scheduler = new TestScheduler();
                    var never = Observable.Never<ITogglDataSource>();
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>()).Returns(never);

                    ViewModel.Password = ValidPassword;
                    ViewModel.NextCommand.Execute();

                    ViewModel.NextCommand.Execute();

                    LoginManager.Received(1).Login(Arg.Any<Email>(), Arg.Any<string>());
                }

                [Fact]
                public void NavigatesToTheTimeEntriesViewModelWhenTheLoginSucceeds()
                {
                    ViewModel.Password = ValidPassword;
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>())
                                .Returns(Observable.Return(Substitute.For<ITogglDataSource>()));

                    ViewModel.NextCommand.Execute();

                    NavigationService.Received().Navigate<TimeEntriesLogViewModel>();
                }

                [Fact]
                public void StopsTheViewModelLoadStateWhenItCompletes()
                {
                    ViewModel.Password = ValidPassword;
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>())
                                .Returns(Observable.Return(Substitute.For<ITogglDataSource>()));

                    ViewModel.NextCommand.Execute();

                    ViewModel.IsLoading.Should().BeFalse();
                }

                [Fact]
                public void StopsTheViewModelLoadStateWhenItErrors()
                {
                    ViewModel.Password = ValidPassword;
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>())
                                .Returns(Observable.Throw<ITogglDataSource>(new Exception()));

                    ViewModel.NextCommand.Execute();

                    ViewModel.IsLoading.Should().BeFalse();
                }

                [Fact]
                public void DoesNotNavigateWhenTheLoginFails()
                {
                    ViewModel.Password = ValidPassword;
                    LoginManager.Login(Arg.Any<Email>(), Arg.Any<string>())
                                .Returns(Observable.Throw<ITogglDataSource>(new Exception()));

                    ViewModel.NextCommand.Execute();

                    NavigationService.DidNotReceive().Navigate<TimeEntriesLogViewModel>();
                }
            }
        }

        public class ThePreviousCommand : LoginViewModelTest
        {
            [Fact]
            public void ReturnsToTheEmailPage()
            {
                ViewModel.Email = ValidEmail;
                ViewModel.NextCommand.Execute();

                ViewModel.BackCommand.Execute();

                ViewModel.CurrentPage.Should().Be(LoginViewModel.EmailPage);
            }

            [Fact]
            public void ClosesTheViewModelWhenInTheEmailPage()
            {
                ViewModel.BackCommand.Execute();

                NavigationService.Received().Close(Arg.Is(ViewModel));
            }
        }
    }
}
