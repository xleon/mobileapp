using System;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Ultrawave;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Exceptions;
using Xunit;
using Microsoft.Reactive.Testing;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class LoginViewModelTests
    {
        public abstract class LoginViewModelTest : BaseViewModelTests<LoginViewModel>
        {
            protected TestScheduler TestScheduler { get; } = new TestScheduler();

            protected IUser User { get; } = new User { Id = 10 };
        }

        public class TheLoginCommand : LoginViewModelTest
        {
            [Fact]
            public void CanNotBeExecutedAgainUntilTheObservableReturns()
            {
                DataSource.User
                          .Login(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(Observable.Return(User).Delay(TimeSpan.FromMinutes(100)));

                ViewModel.LoginCommand.Execute();
            
                ViewModel.LoginCommand.CanExecute().Should().BeFalse();
            }

            [Fact]
            public void CanBeExecutedAgainIfTheObservableReturns()
            {
                DataSource.User
                          .Login(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(Observable.Return(User)
                                             .SubscribeOn(TestScheduler));

                ViewModel.LoginCommand.Execute();
                TestScheduler.Start();

                ViewModel.LoginCommand.CanExecute().Should().BeTrue();
            }

            [Fact]
            public void CanBeExecutedAgainIfTheObservableFails()
            {
                DataSource.User
                          .Login(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(Observable.Throw<IUser>(new ApiException(""))
                                             .SubscribeOn(TestScheduler));

                ViewModel.LoginCommand.Execute();
                TestScheduler.Start();

                ViewModel.LoginCommand.CanExecute().Should().BeTrue();
            }
        }
    }
}
