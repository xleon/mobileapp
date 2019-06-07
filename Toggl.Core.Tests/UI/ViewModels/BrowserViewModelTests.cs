using System;
using FluentAssertions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class BrowserViewModelTests
    {
        public abstract class BrowserViewModelTest : BaseViewModelWithInputTests<BrowserViewModel, BrowserParameters>
        {
            protected override BrowserViewModel CreateViewModel()
                => new BrowserViewModel(NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor : BrowserViewModelTest
        {

            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new BrowserViewModel(
                        useNavigationService ? NavigationService : null,
                        useRxActionFactory ? RxActionFactory : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class ThePrepareMethod : BrowserViewModelTest
        {
            private readonly BrowserParameters parameters = new BrowserParameters
            {
                Title = "Some Title",
                Url = "https://someurl.com"
            };

            [Fact, LogIfTooSlow]
            public void SetsTheUrlProperty()
            {
                ViewModel.Initialize(parameters);

                ViewModel.Url.Should().Be(parameters.Url);
            }

            [Fact, LogIfTooSlow]
            public void SetsTheTitleProperty()
            {
                ViewModel.Initialize(parameters);

                ViewModel.Title.Should().Be(parameters.Title);
            }
        }
    }
}
