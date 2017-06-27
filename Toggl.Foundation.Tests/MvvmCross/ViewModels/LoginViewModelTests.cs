using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Services;
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
            protected IApiFactory ApiFactory { get; } = Substitute.For<IApiFactory>();

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                Ioc.RegisterSingleton(ApiFactory);
            }

            protected override LoginViewModel CreateViewModel()
                => new LoginViewModel(ApiFactory, DataSource);
        }

        public class TheConstructor : LoginViewModelTests
        {
            private static readonly IApiFactory ApiFactory = Substitute.For<IApiFactory>();
            private static readonly ITogglDataSource DataSource = Substitute.For<ITogglDataSource>();
            
            [Theory]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useApiFactory, bool useDataSource)
            {
                var apiFactory = useApiFactory ? ApiFactory : null;
                var dataSource = useDataSource ? DataSource : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new LoginViewModel(apiFactory, dataSource);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
