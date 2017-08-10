using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class StartTimeEntryViewModelTests
    {
        public class StartTimeEntryViewModelTest : BaseViewModelTests<StartTimeEntryViewModel>
        {
            protected override StartTimeEntryViewModel CreateViewModel()
                => new StartTimeEntryViewModel(DataSource, TimeService, NavigationService);
        }

        public class TheConstructor : StartTimeEntryViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService, bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new StartTimeEntryViewModel(dataSource, timeService, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheInitializeMethod : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task SetsTheDateAccordingToTheDateParameterReceived()
            {
                var date = DateTimeOffset.UtcNow;
                var parameter = DateParameter.WithDate(date);

                await ViewModel.Initialize(parameter);

                ViewModel.StartDate.Should().BeSameDateAs(date);
            }
        }

        public class TheBackCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public class TheToggleBillableCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public void TogglesTheIsBillableProperty()
            {
                var expected = !ViewModel.IsBillable;

                ViewModel.ToggleBillableCommand.Execute();

                ViewModel.IsBillable.Should().Be(expected);
            }
        }

        public class TheDoneCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task StartsANewTimeEntry()
            {
                var date = DateTimeOffset.UtcNow;
                var description = "Testing Toggl apps";
                var dateParameter = DateParameter.WithDate(date);

                await ViewModel.Initialize(dateParameter);
                ViewModel.RawTimeEntryText = description;
                ViewModel.DoneCommand.Execute();

                await DataSource.TimeEntries.Received().Start(
                    Arg.Is(dateParameter.GetDate()), 
                    Arg.Is(description),
                    Arg.Is(false)
                );
            }

            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.DoneCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }
    }
}
