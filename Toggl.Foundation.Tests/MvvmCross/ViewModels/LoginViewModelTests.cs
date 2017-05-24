﻿using System;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Ultrawave;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Exceptions;
using Xunit;
using Microsoft.Reactive.Testing;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Ultrawave.Network;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;

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
            protected IApiFactory ApiFactory { get; } = Substitute.For<IApiFactory>();

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                Ioc.RegisterSingleton(ApiFactory);
            }
        }

        public class TheEmailIsValidProperty : LoginViewModelTest
        {
            [Fact]
            public void ReturnsTrueIfTheUsernameIsValid()
            {
                ViewModel.Email = ValidEmail;

                ViewModel.EmailIsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnsFalseIfTheUsernameIsInvalid()
            {
                ViewModel.Email = InvalidEmail;

                ViewModel.EmailIsValid.Should().BeFalse();
            }
        }

        public class TheLoginCommand : LoginViewModelTest
        {
            public TheLoginCommand()
            {
                ViewModel.Email = ValidEmail;
            }

            [Fact]
            public void CanNotBeExecutedWhileTheEmailIsInvalid()
            {
                ViewModel.Email = InvalidEmail;
            
                ViewModel.LoginCommand.CanExecute().Should().BeFalse();
            }

            [Fact]
            public void CanNotBeExecutedAgainUntilTheObservableReturns()
            {
                DataSource.User
                          .Login(Arg.Any<Email>(), Arg.Any<string>())
                          .Returns(Observable.Return(User).Delay(TimeSpan.FromMinutes(100)));

                ViewModel.LoginCommand.Execute();
            
                ViewModel.LoginCommand.CanExecute().Should().BeFalse();
            }

            [Fact]
            public void CanBeExecutedAgainIfTheObservableReturns()
            {
                DataSource.User
                          .Login(Arg.Any<Email>(), Arg.Any<string>())
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
                    .Login(Arg.Any<Email>(), Arg.Any<string>())
                    .Returns(Observable.Throw<IUser>(new ApiException(""))
                        .SubscribeOn(TestScheduler));

                ViewModel.LoginCommand.Execute();
                TestScheduler.Start();

                ViewModel.LoginCommand.CanExecute().Should().BeTrue();
            }

            [Fact]
            public void CreatesANewApiWithTheReturnedUserToken()
            {
                var expectedHeader = Credentials.WithApiToken(User.ApiToken).Header;

                DataSource.User
                    .Login(Arg.Any<Email>(), Arg.Any<string>())
                    .Returns(Observable.Return(User));

                ViewModel.LoginCommand.Execute();

                ApiFactory.Received()
                          .CreateApiWith(Arg.Is<Credentials>(
                              credentials => credentials.Header.Value == expectedHeader.Value)
                          );
            }

            [Fact]
            public void RegistersANewApiWithTheReturnedUserToken()
            {
                var oldClient = Ioc.Resolve<ITogglDataSource>();

                DataSource.User
                    .Login(Arg.Any<Email>(), Arg.Any<string>())
                    .Returns(Observable.Return(User));

                ViewModel.LoginCommand.Execute();

                var actualClient = Ioc.Resolve<ITogglClient>();
                actualClient.Should().NotBe(oldClient);
            }

            [Fact]
            public void RegisterANewDataSourceWithTheReturnedUserToken()
            {
                var oldDataSource = Ioc.Resolve<ITogglDataSource>();
                DataSource.User
                    .Login(Arg.Any<Email>(), Arg.Any<string>())
                    .Returns(Observable.Return(User));

                ViewModel.LoginCommand.Execute();

                var newDataSource = Ioc.Resolve<ITogglDataSource>();
                newDataSource.Should().NotBe(oldDataSource);
            }
        }
    }
}
    