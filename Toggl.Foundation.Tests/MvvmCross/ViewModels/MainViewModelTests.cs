﻿﻿﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;
using TimeEntry = Toggl.Foundation.Models.TimeEntry;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class MainViewModelTests
    {
        public class MainViewModelTest : BaseViewModelTests<MainViewModel>
        {
            protected override MainViewModel CreateViewModel()
                => new MainViewModel(DataSource, TimeService, NavigationService);
        }

        public class TheConstructor : MainViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService, bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MainViewModel(dataSource, timeService, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheViewAppearedMethod : MainViewModelTest
        {
            [Fact]
            public void RequestsTheSuggestionsViewModel()
            {
                ViewModel.ViewAppeared();

                NavigationService.Received().Navigate<SuggestionsViewModel>();
            }

            [Fact]
            public void RequestsTheLogTimeEntriesViewModel()
            {
                ViewModel.ViewAppeared();

                NavigationService.Received().Navigate<TimeEntriesLogViewModel>();
            }
        }

        public class TheStartTimeEntryCommand : MainViewModelTest
        {
            [Fact]
            public async Task NavigatesToTheStartTimeEntryViewModel()
            {
                await ViewModel.StartTimeEntryCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<StartTimeEntryViewModel, DateParameter>(Arg.Any<DateParameter>());
            }

            [Fact]
            public async Task PassesTheCurrentDateToTheStartTimeEntryViewModel()
            {
                var date = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(date);

                await ViewModel.StartTimeEntryCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<StartTimeEntryViewModel, DateParameter>(
                    Arg.Is<DateParameter>(parameter => parameter.DateString == date.ToString())
                );
            }
        }

        public class TheOpenSettingsCommand : MainViewModelTest
        {
            [Fact]
            public async Task NavigatesToTheSettingsViewModel()
            {
                await ViewModel.OpenSettingsCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<SettingsViewModel>();
            }
        }
           
        public class TheStopTimeEntryCommand : MainViewModelTest
        {
            [Fact]
            public async Task CallsTheStopMethodOnTheDataSource()
            {
                var date = DateTimeOffset.UtcNow;
                TimeService.CurrentDateTime.Returns(date);

                await ViewModel.StopTimeEntryCommand.ExecuteAsync();

                await DataSource.TimeEntries.Received().Stop(date);
            }

            [Fact]
            public async Task SetsTheElapsedTimeToZero()
            {
                await ViewModel.StopTimeEntryCommand.ExecuteAsync();

                ViewModel.CurrentTimeEntryElapsedTime.Should().Be(TimeSpan.Zero);
            }
        }

        public class TheCurrentlyRunningTimeEntryProperty : MainViewModelTest
        {
            [Fact]
            public async Task UpdatesWhenTheTimeEntriesSourcesCurrentlyRunningTimeEntryEmitsANewTimeEntry()
            {
                var timeEntry = TimeEntry.Builder.Create(13).SetStart(DateTimeOffset.Now).SetDescription("").Build();
                var subject = new BehaviorSubject<IDatabaseTimeEntry>(null);
                DataSource.TimeEntries.CurrentlyRunningTimeEntry.Returns(subject.AsObservable());

                await ViewModel.Initialize();
                subject.OnNext(timeEntry);

                ViewModel.CurrentlyRunningTimeEntry.Should().Be(timeEntry);
            }
        }
    }
}
