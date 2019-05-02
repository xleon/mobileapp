using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Tests.Generators;
using Toggl.Shared;
using Xunit;
using Toggl.Core.Tests.TestExtensions;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public class SelectDurationFormatViewModelTests
    {
        public abstract class SelectDurationFormatViewModelTest : BaseViewModelTests<SelectDurationFormatViewModel, DurationFormat, DurationFormat>
        {
            protected override SelectDurationFormatViewModel CreateViewModel()
                => new SelectDurationFormatViewModel(NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor : SelectDurationFormatViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                Action tryingToConstructWithEmptyParameters = ()
                    => new SelectDurationFormatViewModel(
                        useNavigationService ? NavigationService : null,
                        useRxActionFactory ? RxActionFactory : null);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheDurationFormatProperty : SelectDurationFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void HasThreeAvailableOptions()
            {
                ViewModel.Initialize(DurationFormat.Classic);

                ViewModel.DurationFormats.Should().HaveCount(3);
                ViewModel.DurationFormats[0].DurationFormat.Should().Be(DurationFormat.Classic);
                ViewModel.DurationFormats[1].DurationFormat.Should().Be(DurationFormat.Improved);
                ViewModel.DurationFormats[2].DurationFormat.Should().Be(DurationFormat.Decimal);
            }

            [Fact, LogIfTooSlow]
            public void TheSelectedOptionIsSelected()
            {
                ViewModel.Initialize(DurationFormat.Improved);

                ViewModel.DurationFormats[0].Selected.Should().BeFalse();
                ViewModel.DurationFormats[1].Selected.Should().BeTrue();
                ViewModel.DurationFormats[2].Selected.Should().BeFalse();
            }
        }

        public sealed class TheCloseAction : SelectDurationFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheDefaultDurationFormat()
            {
                var durationFormat = DurationFormat.Improved;
                
                await ViewModel.Initialize(durationFormat);

                ViewModel.Close.Execute();
                TestScheduler.Start();

                (await ViewModel.ReturnedValue()).Should().Be(durationFormat);
            }
        }

        public sealed class TheSelectDurationFormatAction : SelectDurationFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheDefaultDurationFormat()
            {
                var defaultDuration = DurationFormat.Classic;
                
                await ViewModel.Initialize(defaultDuration);

                var selectedDuration = ViewModel.DurationFormats[1];

                ViewModel.SelectDurationFormat.Execute(selectedDuration);
                TestScheduler.Start();

                (await ViewModel.ReturnedValue()).Should().Be(selectedDuration.DurationFormat);
            }
        }
    }
}
