using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class AboutViewModelTests
    {
        public abstract class AboutViewModelTest : BaseViewModelTests<AboutViewModel>
        {
            protected override AboutViewModel CreateViewModel()
                => new AboutViewModel(NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor : AboutViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new AboutViewModel(
                        navigationService,
                        rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheLicensesCommand : AboutViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void NavigatesToTheLicensesViewModel()
            {
                ViewModel.OpenLicensesView.Execute();

                NavigationService.Received().Navigate<LicensesViewModel>();
            }
        }

        public sealed class TheTermsOfServiceCommand : AboutViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task OpensTheBrowserInTheTermsOfServicePage()
            {
                ViewModel.OpenTermsOfServiceView.Execute();

                NavigationService.Received().Navigate<BrowserViewModel, BrowserParameters>(
                    Arg.Is<BrowserParameters>(parameter => parameter.Url == Resources.TermsOfServiceUrl)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task OpensTheBrowserWithTheAppropriateTitle()
            {
                ViewModel.OpenTermsOfServiceView.Execute();

                NavigationService.Received().Navigate<BrowserViewModel, BrowserParameters>(
                    Arg.Is<BrowserParameters>(parameter => parameter.Title == Resources.TermsOfService)
                );
            }
        }

        public sealed class ThePrivacyPolicyCommand : AboutViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task OpensTheBrowserInThePrivacyPolicyPage()
            {
                ViewModel.OpenPrivacyPolicyView.Execute();

                NavigationService.Received().Navigate<BrowserViewModel, BrowserParameters>(
                    Arg.Is<BrowserParameters>(parameter => parameter.Url == Resources.PrivacyPolicyUrl)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task OpensTheBrowserWithTheAppropriateTitle()
            {
                ViewModel.OpenPrivacyPolicyView.Execute();

                NavigationService.Received().Navigate<BrowserViewModel, BrowserParameters>(
                    Arg.Is<BrowserParameters>(parameter => parameter.Title == Resources.PrivacyPolicy)
                );
            }
        }
    }
}
