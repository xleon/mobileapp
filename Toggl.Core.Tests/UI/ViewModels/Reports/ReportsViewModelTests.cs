﻿using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels.DateRangePicker;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.Views;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
using Toggl.Core.Analytics;
using DateRangePeriod = Toggl.Core.Models.DateRangePeriod;
using System.Reactive.Subjects;
using Toggl.Core.UI.Helper;
using System.Globalization;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    using static Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel;
    using WorkspaceOptions = IEnumerable<SelectOption<IThreadSafeWorkspace>>;

    public class ReportsViewModelTests
    {
        public abstract class ReportsViewModelBaseTest : BaseViewModelTests<ReportsViewModel>
        {
            private List<MockWorkspace> workspaces;

            protected new IDateRangeShortcutsService DateRangeShortcutsService { get; private set; } = Substitute.For<IDateRangeShortcutsService>();

            protected new ReportsViewModel ViewModel { get; set; }

            protected void SetupEnvironment(
                Func<List<MockWorkspace>, IEnumerable<MockWorkspace>> adjustWorkspaces = null,
                Func<List<MockWorkspace>, MockWorkspace> setDefaultWorkspace = null,
                Func<WorkspaceOptions, IThreadSafeWorkspace> dialogWorkspaceSelector = null,
                Func<DateFormat> dateFormatSelector = null,
                Func<DateRange?> selectedDateRangeSelector = null,
                Func<int> unsyncedProjectsCountSelector = null,
                Action beforeViewModelCreation = null)
            {
                workspaces = Enumerable.Range(0, 10)
                    .Select(id => new MockWorkspace(id, isInaccessible: id % 4 == 0))
                    .ToList();

                adjustWorkspaces = adjustWorkspaces ?? (ws => ws);
                workspaces = adjustWorkspaces(workspaces).ToList();

                setDefaultWorkspace = setDefaultWorkspace ?? (ws => ws.First());
                var defaultWorkspace = setDefaultWorkspace(workspaces);

                selectedDateRangeSelector = selectedDateRangeSelector
                    ?? (() => new DateRange(DateTime.Parse("2019-01-01"), DateTime.Parse("2019-01-08")));
                var selectedRange = selectedDateRangeSelector();

                unsyncedProjectsCountSelector = unsyncedProjectsCountSelector ?? (() => 0);
                var unsyncedProjectsCount = unsyncedProjectsCountSelector();

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(defaultWorkspace));

                InteractorFactory
                    .GetAllWorkspaces()
                    .Execute()
                    .Returns(Observable.Return(workspaces));

                InteractorFactory
                    .ObserveAllWorkspaces()
                    .Execute()
                    .Returns(Observable.Return(workspaces));

                var userObservable = Observable.Return(new MockUser { Id = 1, BeginningOfWeek = BeginningOfWeek.Wednesday });

                InteractorFactory
                    .GetCurrentUser()
                    .Execute()
                    .Returns(userObservable);

                DataSource.User
                    .Current
                    .Returns(userObservable);

                TimeService
                    .CurrentDateTime
                    .Returns(new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero));

                DateRangeShortcutsService = new DateRangeShortcutsService(DataSource, TimeService);

                var totals = new TimeEntriesTotals();
                InteractorFactory
                    .GetReportsTotals(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<DateTimeOffsetRange>())
                    .Execute()
                    .Returns(Observable.Return(totals));

                var summaryData = new ProjectSummaryReport(Array.Empty<ChartSegment>(), unsyncedProjectsCount);
                InteractorFactory
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset?>())
                    .Execute()
                    .Returns(Observable.Return(summaryData));

                dialogWorkspaceSelector = dialogWorkspaceSelector ?? (ws => ws.First().Item);
                View.Select(Arg.Any<string>(), Arg.Any<WorkspaceOptions>(), Arg.Any<int>())
                    .Returns(c =>
                    {
                        var options = c.ArgAt<WorkspaceOptions>(1);
                        var chosenElement = dialogWorkspaceSelector(options);
                        return Observable.Return(chosenElement);
                    });

                dateFormatSelector = dateFormatSelector ?? (() => DateFormat.FromLocalizedDateFormat("YYYY-MM-DD"));
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(dateFormatSelector());
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));

                NavigationService
                    .Navigate<DateRangePickerViewModel, Either<DateRangePeriod, DateRange>, DateRangeSelectionResult>(
                        Arg.Any<Either<DateRangePeriod, DateRange>>(), Arg.Any<IView>())
                    .Returns(new DateRangeSelectionResult(selectedRange, DateRangeSelectionSource.Calendar));

                beforeViewModelCreation?.Invoke();

                ViewModel = CreateViewModel();
                ViewModel.AttachView(View);
            }

            protected override ReportsViewModel CreateViewModel()
                => new ReportsViewModel(
                    DataSource,
                    NavigationService,
                    InteractorFactory,
                    SchedulerProvider,
                    RxActionFactory,
                    AnalyticsService,
                    TimeService,
                    DateRangeShortcutsService);
        }

        public sealed class TheConstructor : ReportsViewModelBaseTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useNavigationService,
                bool useSchedulerProvider,
                bool useInteractorFactory,
                bool useRxActionFactory,
                bool useAnalyticsService,
                bool useTimeService,
                bool useDateRangeShortcutsService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var timeService = useTimeService ? TimeService : null;
                var dateRangeShortcutsService = useDateRangeShortcutsService ? DateRangeShortcutsService : null;

                Action tryingToConstructWithEmptyParameters = () => new ReportsViewModel(
                    dataSource,
                    navigationService,
                    interactorFactory,
                    schedulerProvider,
                    rxActionFactory,
                    analyticsService,
                    timeService,
                    dateRangeShortcutsService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheElementsProperty : ReportsViewModelBaseTest
        {
            private bool isLoadingElements(IEnumerable<IReportElement> elements)
                => elements.Cast<ReportElementBase>().All(element => element.IsLoading);

            private bool isNotLoadingElements(IEnumerable<IReportElement> elements)
                => elements.OfType<ReportElementBase>().All(element => !element.IsLoading);

            private bool isNoDataElement(IEnumerable<IReportElement> elements)
                => elements.OfType<ReportNoDataElement>().Count() == 1;

            [Fact, LogIfTooSlow]
            public async Task EmitsElementsForDefaultWorkspaceAndDefaultTimeRangeOnViewModelCreation()
            {
                SetupEnvironment();
                var observer = TestScheduler.CreateObserver<IEnumerable<IReportElement>>();

                await ViewModel.Initialize();
                ViewModel.Elements.Subscribe(observer);
                TestScheduler.Start();

                observer.Messages.AssertEqual(
                    OnNext<IEnumerable<IReportElement>>(1, isLoadingElements),
                    OnNext<IEnumerable<IReportElement>>(2, isNotLoadingElements)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsLoadingElementsWhenWorkspaceChangesAndNoDataIsPresent()
            {
                SetupEnvironment();
                var observer = TestScheduler.CreateObserver<IEnumerable<IReportElement>>();

                await ViewModel.Initialize();
                ViewModel.Elements.Subscribe(observer);
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                observer.Messages.AssertEqual(
                    // Default data base upon initial filter
                    OnNext<IEnumerable<IReportElement>>(1, isLoadingElements),
                    OnNext<IEnumerable<IReportElement>>(2, isNotLoadingElements),

                    // Data based on the workspace change
                    OnNext<IEnumerable<IReportElement>>(3, isLoadingElements),
                    OnNext<IEnumerable<IReportElement>>(4, isNoDataElement)
                );
            }
        }

        public sealed class TheFormattedTimeRangeProperty : ReportsViewModelBaseTest
        {
            private string removeDropDownCharacter(string dateRange) => dateRange[0..^2].Trim();

            [Fact, LogIfTooSlow]
            public async Task EmitsInitialTimeRange()
            {
                SetupEnvironment();
                var observer = TestScheduler.CreateObserver<string>();

                await ViewModel.Initialize();
                ViewModel.FormattedTimeRange.Select(removeDropDownCharacter).Subscribe(observer);
                TestScheduler.Start();

                observer.FirstEmittedValue().Should().Be(Resources.ThisWeek);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsCorrectFormattedDateRangeWhenDateRangeIsSelected()
            {
                SetupEnvironment();
                var observer = TestScheduler.CreateObserver<string>();

                await ViewModel.Initialize();
                ViewModel.FormattedTimeRange.Select(removeDropDownCharacter).Subscribe(observer);
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be("01-01 - 01-08");
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsWhenDateRangeSelectionIsCanceled()
            {
                SetupEnvironment(selectedDateRangeSelector: () => null);

                var observer = TestScheduler.CreateObserver<string>();

                await ViewModel.Initialize();
                ViewModel.FormattedTimeRange.Select(removeDropDownCharacter).Subscribe(observer);
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                observer.Messages.Count().Should().Be(1);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsWhenDateDateFormatIsChanged()
            {
                var initialFormat = DateFormat.ValidDateFormats.First();
                var changedFormat = DateFormat.ValidDateFormats.Last();
                var initialPreferences = Substitute.For<IThreadSafePreferences>();
                var changedPreferences = Substitute.For<IThreadSafePreferences>();
                initialPreferences.DateFormat.Returns(initialFormat);
                changedPreferences.DateFormat.Returns(changedFormat);
                var preferences = new BehaviorSubject<IThreadSafePreferences>(initialPreferences);
                SetupEnvironment(beforeViewModelCreation: () =>
                    DataSource.Preferences.Current.Returns(preferences.AsObservable())
                );
                var observer = TestScheduler.CreateObserver<string>();

                await ViewModel.Initialize();
                ViewModel.FormattedTimeRange.Select(removeDropDownCharacter).Subscribe(observer);
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                preferences.OnNext(changedPreferences);
                TestScheduler.Start();

                observer.Messages.AssertEqual(
                    OnNext(1, Resources.ThisWeek),
                    OnNext(2, "01/01 - 01/08"),
                    OnNext(3, "01.01 - 08.01")
               );
            }
        }

        public sealed class TheHasMultipleWorkspacesProperty : ReportsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTrueForMultipleWorkspaces()
            {
                SetupEnvironment();
                var observer = TestScheduler.CreateObserver<bool>();

                await ViewModel.Initialize();
                ViewModel.HasMultipleWorkspaces.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFalseForSingleWorkspace()
            {
                SetupEnvironment(adjustWorkspaces: workspaces => workspaces.Where(ws => !ws.IsInaccessible).Take(1));
                var observer = TestScheduler.CreateObserver<bool>();

                await ViewModel.Initialize();
                ViewModel.HasMultipleWorkspaces.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task CountsOnlyAccessibleWorkspaces()
            {
                SetupEnvironment(
                    adjustWorkspaces: workspaces =>
                    {
                        workspaces.ForEach(ws => ws.IsInaccessible = true);
                        workspaces.First().IsInaccessible = false;
                        return workspaces;
                    });
                var observer = TestScheduler.CreateObserver<bool>();

                await ViewModel.Initialize();
                ViewModel.HasMultipleWorkspaces.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheSelectWorkspaceAction : ReportsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task CallsGetAllWorkspace()
            {
                SetupEnvironment();

                await ViewModel.Initialize();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                await InteractorFactory.GetAllWorkspaces()
                    .Received()
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task OpensViewSelectorForWorkspace()
            {
                SetupEnvironment();

                await ViewModel.Initialize();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                await View.Received().Select(Arg.Any<string>(), Arg.Any<WorkspaceOptions>(), Arg.Any<int>());
            }

            [Fact, LogIfTooSlow]
            public async Task OpensViewSelectorOnlyWithAccessibleWorkspace()
            {
                var expectedWorkspaceCount = 0;
                SetupEnvironment(adjustWorkspaces: workspaces =>
                {
                    expectedWorkspaceCount = workspaces.Count(ws => !ws.IsInaccessible);
                    return workspaces;
                });

                await ViewModel.Initialize();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                await View.Received().Select(
                    Arg.Any<string>(),
                    Arg.Is<WorkspaceOptions>(ws => ws.Count() == expectedWorkspaceCount),
                    Arg.Any<int>());
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsCorrectWorkspace()
            {
                var chosenWorkspace = (IThreadSafeWorkspace)null;
                SetupEnvironment(dialogWorkspaceSelector: workspaces =>
                {
                    chosenWorkspace = workspaces.Where(ws => !ws.Item.IsInaccessible).Last().Item;
                    return chosenWorkspace;
                });
                var observer = TestScheduler.CreateObserver<IThreadSafeWorkspace>();
                ViewModel.SelectWorkspace.Elements.Subscribe(observer);

                await ViewModel.Initialize();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(chosenWorkspace);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsAnalyticsEvent()
            {
                var unsyncedProjects = 2;
                var initialSelectionDays = 7;
                var initialSource = DateRangeSelectionSource.Initial;
                SetupEnvironment(unsyncedProjectsCountSelector: () => unsyncedProjects);

                await ViewModel.Initialize();
                ViewModel.Elements.Subscribe();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                AnalyticsService.ReportsSuccess.Received().Track(
                    Arg.Is(initialSource),
                    Arg.Is(initialSelectionDays),
                    Arg.Is(unsyncedProjects),
                    Arg.Any<double>());
            }
        }

        public sealed class TheSelectTimeRangeAction : ReportsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheDateRangePicker()
            {
                SetupEnvironment();

                await ViewModel.Initialize();
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                await NavigationService.Received().Navigate<DateRangePickerViewModel, Either<DateRangePeriod, DateRange>, DateRangeSelectionResult>(
                    Arg.Any<Either<DateRangePeriod, DateRange>>(), Arg.Any<IView>());
            }

            [Fact, LogIfTooSlow]
            public async Task OutputsCorrectDate()
            {
                var range = new DateRange(DateTime.Parse("2018-02-06"), DateTime.Parse("2018-04-17"));
                var expectedResult = new DateRangeSelectionResult(range, DateRangeSelectionSource.Calendar);
                SetupEnvironment(selectedDateRangeSelector: () => range);
                var observer = TestScheduler.CreateObserver<DateRangeSelectionResult>();

                await ViewModel.Initialize();
                ViewModel.SelectTimeRange.Elements.Subscribe(observer);
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                var result = observer.LastEmittedValue();
                result.Source.Should().Be(expectedResult.Source);
                result.SelectedRange.Should().Be(expectedResult.SelectedRange);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsAnalyticsEvent()
            {
                var unsyncedProjects = 2;
                var range = new DateRange(DateTime.Parse("2018-02-06"), DateTime.Parse("2018-02-09"));
                var expectedResult = new DateRangeSelectionResult(range, DateRangeSelectionSource.Calendar);

                SetupEnvironment(
                    selectedDateRangeSelector: () => range,
                    unsyncedProjectsCountSelector: () => unsyncedProjects);

                await ViewModel.Initialize();
                ViewModel.Elements.Subscribe();
                ViewModel.SelectTimeRange.Execute();
                TestScheduler.Start();

                AnalyticsService.ReportsSuccess.Received().Track(
                    Arg.Is(expectedResult.Source),
                    Arg.Is(range.Length),
                    Arg.Is(unsyncedProjects),
                    Arg.Any<double>());
            }
        }
    }
}
