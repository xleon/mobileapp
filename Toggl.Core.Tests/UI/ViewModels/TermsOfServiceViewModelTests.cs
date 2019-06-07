using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class TermsOfServiceViewModelTests
    {
        public abstract class TermsOfServiceViewModelTest : BaseViewModelWithOutputTests<TermsOfServiceViewModel, bool>
        {
            protected override TermsOfServiceViewModel CreateViewModel()
                => new TermsOfServiceViewModel(BrowserService, RxActionFactory, NavigationService);
        }

        public sealed class TheConstructor : TermsOfServiceViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useBrowserService,
                bool useRxActionFactory,
                bool useNavigationService)
            {
                var browserService = useBrowserService ? BrowserService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new TermsOfServiceViewModel(browserService, rxActionFactory, navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheViewTermsOfServiceCommand : TermsOfServiceViewModelTest
        {
            private const string termsOfServiceUrl = "https://toggl.com/legal/terms/";

            [Fact, LogIfTooSlow]
            public async void OpensTermsOfService()
            {
                ViewModel.ViewTermsOfService.Execute();
                TestScheduler.Start();

                BrowserService.Received().OpenUrl(termsOfServiceUrl);
            }
        }

        public sealed class TheViewPrivacyPolicyCommand : TermsOfServiceViewModelTest
        {
            private const string privacyPolicyUrl = "https://toggl.com/legal/privacy/";

            [Fact, LogIfTooSlow]
            public async void OpensPrivacyPolicy()
            {
                ViewModel.ViewPrivacyPolicy.Execute();
                TestScheduler.Start();

                BrowserService.Received().OpenUrl(privacyPolicyUrl);
            }
        }
    }
}
