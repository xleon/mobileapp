using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.Foundation.Models;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class TimeEntriesLogViewModelTests
    {
        public class TimeEntriesLogViewModelTest : BaseViewModelTests<TimeEntriesLogViewModel>
        {
            protected override TimeEntriesLogViewModel CreateViewModel()
                => new TimeEntriesLogViewModel(DataSource);
        }

        public class TheConstructor : TimeEntriesLogViewModelTest
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntriesLogViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheEmptyStateTitleProperty : TimeEntriesLogViewModelTest
        {
            [Fact]
            public void ReturnsTheWelcomeStringIfTheIsWelcomePropertyIsTrue()
            {
                ViewModel.IsWelcome = true;

                ViewModel.EmptyStateTitle.Should().Be(Resources.TimeEntriesLogEmptyStateWelcomeTitle);
            }

            [Fact]
            public void ReturnsTheDefaultStringIfTheIsWelcomePropertyIsFalse()
            {
                ViewModel.IsWelcome = false;

                ViewModel.EmptyStateTitle.Should().Be(Resources.TimeEntriesLogEmptyStateTitle);
            }
        }

        public class TheEmptyStateTextProperty : TimeEntriesLogViewModelTest
        {
            [Fact]
            public void ReturnsTheWelcomeStringIfTheIsWelcomePropertyIsTrue()
            {
                ViewModel.IsWelcome = true;

                ViewModel.EmptyStateText.Should().Be(Resources.TimeEntriesLogEmptyStateWelcomeText);
            }

            [Fact]
            public void ReturnsTheDefaultStringIfTheIsWelcomePropertyIsFalse()
            {
                ViewModel.IsWelcome = false;

                ViewModel.EmptyStateText.Should().Be(Resources.TimeEntriesLogEmptyStateText);
            }
        }

        public class TheTimeEntriesProperty
        {
            [Property]
            public Property ShouldBeOrderedAfterInitialization()
            {
                var generator = ViewModelGenerators.ForTimeEntriesLogViewModel(_ => true).ToArbitrary();
                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();

                    for (int i = 1; i < viewModel.TimeEntries.Count(); i++)
                    {
                        var dateTime1 = viewModel.TimeEntries.ElementAt(i - 1).Date;
                        var dateTime2 = viewModel.TimeEntries.ElementAt(i).Date;
                        dateTime1.Should().BeAfter(dateTime2);
                    }
                });
            }

            [Property]
            public Property ShouldNotHaveEmptyGroups()
            {
                var generator = ViewModelGenerators.ForTimeEntriesLogViewModel(_ => true).ToArbitrary();
                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();

                    foreach (var grouping in viewModel.TimeEntries)
                    {
                        grouping.Count().Should().BeGreaterOrEqualTo(1);
                    }
                });
            }

            [Property]
            public Property ShouldHaveOrderedGroupsAfterInitialization()
            {
                var generator =
                    ViewModelGenerators
                        .ForTimeEntriesLogViewModel(m => m == DateTime.UtcNow.Month).ToArbitrary();

                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();
                    foreach (var grouping in viewModel.TimeEntries)
                    {
                        for (int i = 1; i < grouping.Count(); i++)
                        {
                            var dateTime1 = grouping.ElementAt(i - 1).Start;
                            var dateTime2 = grouping.ElementAt(i).Start;
                            dateTime1.Should().BeOnOrAfter(dateTime2);
                        }
                    }
                });
            }

            public abstract class TimeEntryDataSourceObservableTest : TimeEntriesLogViewModelTest
            {
                protected const int InitialAmountOfTimeEntries = 20;
                
                protected Subject<IDatabaseTimeEntry> TimeEntryCreatedSubject = new Subject<IDatabaseTimeEntry>();
                protected Subject<IDatabaseTimeEntry> TimeEntryUpdatedSubject = new Subject<IDatabaseTimeEntry>();
                protected IDatabaseTimeEntry NewTimeEntry =
                    TimeEntry.Builder.Create(21)
                             .SetStart(DateTimeOffset.UtcNow)
                             .SetDescription("")
                             .Build();

                protected TimeEntryDataSourceObservableTest()
                {
                    var startTime = DateTimeOffset.UtcNow.AddHours(-2);

                    var observable = Enumerable.Range(1, InitialAmountOfTimeEntries)
                              .Select(i => TimeEntry.Builder.Create(i))
                              .Select(builder => builder.SetStart(startTime).SetDescription("").Build())
                              .Select(te => te.With(startTime.AddHours(2)))
                              .Apply(Observable.Return);

                    DataSource.TimeEntries.GetAll().Returns(observable);
                    DataSource.TimeEntries.TimeEntryCreated.Returns(TimeEntryCreatedSubject.AsObservable());
                    DataSource.TimeEntries.TimeEntryUpdated.Returns(TimeEntryUpdatedSubject.AsObservable());
                }
            }

            public class WhenReceivingAnEventFromTheTimeEntryCreatedObservable : TimeEntryDataSourceObservableTest
            {
                [Fact]
                public async ThreadingTask AddsTheCreatedTimeEntryToTheList()
                {
                    await ViewModel.Initialize();
                    var newTimeEntry = NewTimeEntry.With(DateTimeOffset.UtcNow.AddHours(1));

                    TimeEntryCreatedSubject.OnNext(newTimeEntry);

                    ViewModel.TimeEntries.Any(c => c.Any(te => te.Id == 21)).Should().BeTrue();
                    ViewModel.TimeEntries.Aggregate(0, (acc, te) => acc + te.Count).Should().Be(InitialAmountOfTimeEntries + 1);
                }

                [Fact]
                public async ThreadingTask IgnoresTheTimeEntryIfItsStillRunning()
                {
                    await ViewModel.Initialize();

                    TimeEntryCreatedSubject.OnNext(NewTimeEntry);

                    ViewModel.TimeEntries.Any(c => c.Any(te => te.Id == 21)).Should().BeFalse();
                    ViewModel.TimeEntries.Aggregate(0, (acc, te) => acc + te.Count).Should().Be(InitialAmountOfTimeEntries);
                }
            }

            public class WhenReceivingAnEventFromTheTimeEntryUpdatedObservable : TimeEntryDataSourceObservableTest
            {
                [Fact]
                //This can happen, for example, if the time entry was just stopped
                public async ThreadingTask AddsTheTimeEntryIfItWasNotAddedPreviously()
                {
                    var stopDate = DateTimeOffset.UtcNow.AddHours(1);
                    await ViewModel.Initialize();
                    var newTimeEntry = NewTimeEntry.With(stopDate);

                    TimeEntryUpdatedSubject.OnNext(newTimeEntry);

                    ViewModel.TimeEntries.Any(c => c.Any(te => te.Id == 21)).Should().BeTrue();
                    ViewModel.TimeEntries.Aggregate(0, (acc, te) => acc + te.Count).Should().Be(InitialAmountOfTimeEntries + 1);
                }

                [Fact]
                public async ThreadingTask IgnoresTheTimeEntryIfItWasDeleted()
                {
                    await ViewModel.Initialize();
                    var newTimeEntry = TimeEntry.DirtyDeleted(NewTimeEntry.With(DateTimeOffset.UtcNow.AddHours(1)));

                    TimeEntryCreatedSubject.OnNext(NewTimeEntry);

                    ViewModel.TimeEntries.Any(c => c.Any(te => te.Id == 21)).Should().BeFalse();
                    ViewModel.TimeEntries.Aggregate(0, (acc, te) => acc + te.Count).Should().Be(InitialAmountOfTimeEntries);
                }
            }
        }
    }
}
