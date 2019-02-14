using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Helper;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog;
using Toggl.Foundation.Services;
using Toggl.Foundation.Tests.Generators;
using Toggl.Foundation.Tests.Mocks;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    using TimeEntriesLog = IEnumerable<CollectionSection<DaySummaryViewModel, LogItemViewModel>>;

    public sealed class TimeEntriesViewModelTests
    {
        public abstract class TimeEntriesViewModelTest
        {
            protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
            protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();
            protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();
            protected TestSchedulerProvider SchedulerProvider { get; } = new TestSchedulerProvider();
            protected IRxActionFactory RxActionFactory { get; }
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();

            protected TimeEntriesViewModel ViewModel { get; private set; }

            protected TimeEntriesViewModel CreateViewModel()
                => new TimeEntriesViewModel(DataSource, InteractorFactory, AnalyticsService, SchedulerProvider, RxActionFactory, TimeService);

            protected TimeEntriesViewModelTest()
            {
                RxActionFactory = new RxActionFactory(SchedulerProvider);
                ViewModel = CreateViewModel();
            }
        }

        public sealed class TheConstructor : TimeEntriesViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useInteractorFactory,
                bool useAnalyticsService,
                bool useSchedulerProvider,
                bool useRxActionFactory,
                bool useTimeService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntriesViewModel(dataSource, interactorFactory, analyticsService, schedulerProvider, rxActionFactory, timeService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public abstract class TimeEntryDataSourceObservableTest : TimeEntriesViewModelTest
        {
            private static readonly DateTimeOffset now = new DateTimeOffset(2017, 01, 19, 07, 10, 00, TimeZoneInfo.Local.GetUtcOffset(new DateTime(2017, 01, 19)));

            protected const int InitialAmountOfTimeEntries = 20;

            protected ISubject<IEnumerable<IThreadSafeTimeEntry>> TimeEntries { get; }

            protected IThreadSafeTimeEntry NewTimeEntry(long? duration = null)
            {
                return new MockTimeEntry
                {
                    Id = 21,
                    UserId = 10,
                    WorkspaceId = 12,
                    Description = "",
                    At = now,
                    Duration = duration,
                    Start = now,
                    TagIds = new long[] { 1, 2, 3 },
                    Workspace = new MockWorkspace()
                };
            }

            protected TimeEntryDataSourceObservableTest()
            {
                var startTime = now.AddHours(-2);

                var initialTimeEntries =
                    Enumerable.Range(1, InitialAmountOfTimeEntries)
                        .Select(i =>
                            new MockTimeEntry
                            {
                                Id = i,
                                Start = startTime.AddHours(i * 2),
                                Duration = (long)TimeSpan.FromHours(i * 2 + 2).TotalSeconds,
                                UserId = 11,
                                WorkspaceId = 12,
                                Description = "",
                                At = now,
                                TagIds = new long[] { 1, 2, 3 },
                                Workspace = new MockWorkspace { IsInaccessible = false }
                            }
                        );

                TimeEntries = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(initialTimeEntries);
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(TimeEntries);
            }
        }

        public sealed class TheDelayDeleteTimeEntryAction : TimeEntriesViewModelTest
        {
            private readonly TimeEntriesViewModel viewModel;
            private readonly long id = 1235900;
            private readonly IObserver<bool> observer = Substitute.For<IObserver<bool>>();

            public TheDelayDeleteTimeEntryAction()
            {
                viewModel = new TimeEntriesViewModel(DataSource, InteractorFactory, AnalyticsService, SchedulerProvider, RxActionFactory, TimeService);
                viewModel.ShouldShowUndo.Subscribe(observer);
            }

            [Fact]
            public async ThreadingTask ShowsTheUndoUI()
            {
                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.Start();

                observer.Received().OnNext(true);
            }

            [Fact]
            public async ThreadingTask DoesNotHideTheUndoUITooEarly()
            {
                var observable = viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks - 1);
                await observable;

                observer.Received().OnNext(true);
                observer.DidNotReceive().OnNext(false);
            }

            [Fact]
            public async ThreadingTask HidesTheUndoUIAfterSeveralSeconds()
            {
                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.Start();

                observer.Received().OnNext(true);
                observer.Received().OnNext(false);
            }

            [Fact]
            public async ThreadingTask DoesNotHideTheUndoUIIfAnotherItemWasDeletedWhileWaiting()
            {
                var timeEntryA = 1;
                var timeEntryB = 2;

                viewModel.DelayDeleteTimeEntry.Execute(timeEntryA);
                SchedulerProvider.TestScheduler.AdvanceBy((long)(Constants.UndoTime.Ticks * 0.5));
                viewModel.DelayDeleteTimeEntry.Execute(timeEntryB);
                SchedulerProvider.TestScheduler.AdvanceBy((long)(Constants.UndoTime.Ticks * 0.6));

                observer.Received().OnNext(true);
            }

            [Fact]
            public async ThreadingTask ImmediatelyHardDeletesTheTimeEntryWhenAnotherTimeEntryIsDeletedBeforeTheUndoTimeRunsOut()
            {
                var timeEntryA = 1;
                var timeEntryB = 2;

                var observableA = viewModel.DelayDeleteTimeEntry.Execute(timeEntryA);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 2);
                var observableB = viewModel.DelayDeleteTimeEntry.Execute(timeEntryB);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 4);

                InteractorFactory.Received().DeleteTimeEntry(timeEntryA).Execute();
            }

            [Fact]
            public async ThreadingTask ImmediatelyHardDeletesTheTimeEntryWhenCallingFinilizeDelayDeleteTimeEntryIfNeeded()
            {
                var timeEntryA = 1;

                var observableA = viewModel.DelayDeleteTimeEntry.Execute(timeEntryA);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 2);
                await viewModel.FinilizeDelayDeleteTimeEntryIfNeeded();
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 4);

                InteractorFactory.Received().DeleteTimeEntry(timeEntryA).Execute();
            }

            [Fact]
            public async ThreadingTask DoesNotHardDeletesTheTimeEntryWhenNotCallingFinilizeDelayDeleteTimeEntryIfNeeded()
            {
                var timeEntryA = 1;

                var observableA = viewModel.DelayDeleteTimeEntry.Execute(timeEntryA);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 2);

                InteractorFactory.DidNotReceive().DeleteTimeEntry(timeEntryA).Execute();
            }

            [Fact]
            public void ReloadsTimeEntriesLog()
            {
                var observer = SchedulerProvider.TestScheduler.CreateObserver<TimeEntriesLog>();
                viewModel.TimeEntries.Subscribe(observer);

                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.Start();

                // 1 - the initial load of the data
                // 2 - undo bar shown
                // 3 - undo bar hidden
                observer.Messages.Should().HaveCount(3);
            }
        }

        public sealed class TheCancelDeleteTimeEntryAction : TimeEntriesViewModelTest
        {
            private readonly TimeEntriesViewModel viewModel;
            private readonly long id = 123579;
            private readonly IObserver<bool> observer = Substitute.For<IObserver<bool>>();

            public TheCancelDeleteTimeEntryAction()
            {
                viewModel = new TimeEntriesViewModel(DataSource, InteractorFactory, AnalyticsService, SchedulerProvider, RxActionFactory, TimeService);
                viewModel.ShouldShowUndo.Subscribe(observer);
            }

            [Fact]
            public async ThreadingTask DoesNotDeleteTheTimeEntryIfTheUndoIsInitiatedBeforeTheUndoPeriodIsOver()
            {
                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 2);
                viewModel.CancelDeleteTimeEntry.Execute();
                SchedulerProvider.TestScheduler.Start();

                InteractorFactory.DidNotReceive().DeleteTimeEntry(id);
            }

            [Fact]
            public async ThreadingTask DeletesTheTimeEntryIfTheUndoIsInitiatedAfterTheUndoPeriodIsOver()
            {
                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks);
                viewModel.CancelDeleteTimeEntry.Execute();

                InteractorFactory.Received().DeleteTimeEntry(id);
            }

            [Fact]
            public async ThreadingTask HidesTheUndoUI()
            {
                viewModel.DelayDeleteTimeEntry.Execute(id);
                SchedulerProvider.TestScheduler.AdvanceBy(Constants.UndoTime.Ticks / 2);
                viewModel.CancelDeleteTimeEntry.Execute();
                SchedulerProvider.TestScheduler.Start();

                observer.Received().OnNext(true);
                observer.Received().OnNext(false);
            }
        }
    }
}
