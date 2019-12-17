using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.Views;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    using WorkspaceOptions = IEnumerable<SelectOption<IThreadSafeWorkspace>>;

    public class ReportsViewModelTests
    {
        public abstract class ReportsViewModelTest : BaseViewModelTests<ReportsViewModel>
        {
            private List<MockWorkspace> workspaces;

            protected new ICalendarShortcutsService CalendarShortcutsService { get; private set; } = Substitute.For<ICalendarShortcutsService>();

            protected new ReportsViewModel ViewModel { get; set; }

            protected void SetupEnvironment(
                Func<List<MockWorkspace>, IEnumerable<MockWorkspace>> adjustWorkspaces = null,
                Func<List<MockWorkspace>, MockWorkspace> setDefaultWorkspace = null,
                Func<WorkspaceOptions, IThreadSafeWorkspace> dialogWorkspaceSelection = null,
                Func<DateFormat> selectDateFormat = null)
            {
                workspaces = Enumerable.Range(0, 10)
                    .Select(id => new MockWorkspace(id, isInaccessible: id % 4 == 0))
                    .ToList();

                adjustWorkspaces = adjustWorkspaces ?? (ws => ws);
                workspaces = adjustWorkspaces(workspaces).ToList();

                setDefaultWorkspace = setDefaultWorkspace ?? (ws => ws.First());
                var defaultWorkspace = setDefaultWorkspace(workspaces);

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

                CalendarShortcutsService = new CalendarShortcutsService(DataSource, TimeService);

                var totals = new TimeEntriesTotals();
                InteractorFactory
                    .GetReportsTotals(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<DateTimeOffsetRange>())
                    .Execute()
                    .Returns(Observable.Return(totals));

                var summaryData = new ProjectSummaryReport(Array.Empty<ChartSegment>(), 0);
                InteractorFactory
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset?>())
                    .Execute()
                    .Returns(Observable.Return(summaryData));

                dialogWorkspaceSelection = dialogWorkspaceSelection ?? (ws => ws.First().Item);
                View.Select(Arg.Any<string>(), Arg.Any<WorkspaceOptions>(), Arg.Any<int>())
                    .Returns(c =>
                    {
                        var options = c.ArgAt<WorkspaceOptions>(1);
                        var chosenElement = dialogWorkspaceSelection(options);
                        return Observable.Return(chosenElement);
                    });

                selectDateFormat = selectDateFormat ?? (() => DateFormat.FromLocalizedDateFormat("YYYY-MM-DD"));
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(selectDateFormat());
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));

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
                    CalendarShortcutsService);
        }

        public sealed class TheConstructor : ReportsViewModelTest
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
                bool useCalendarShortcutsService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var timeService = useTimeService ? TimeService : null;
                var calendarShortcutsService = useCalendarShortcutsService ? CalendarShortcutsService : null;

                Action tryingToConstructWithEmptyParameters = () => new ReportsViewModel(
                    dataSource,
                    navigationService,
                    interactorFactory,
                    schedulerProvider,
                    rxActionFactory,
                    analyticsService,
                    timeService,
                    calendarShortcutsService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheElementsProperty : ReportsViewModelTest
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

        public sealed class TheFormattedTimeRangeProperty : ReportsViewModelTest
        {
            // TODO Add tests when the time range selector view model is implemented
        }

        public sealed class TheHasMultipleWorkspacesProperty : ReportsViewModelTest
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

        public sealed class TheSelectWorkspaceAction : ReportsViewModelTest
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
                SetupEnvironment(dialogWorkspaceSelection: workspaces =>
                {
                    chosenWorkspace = workspaces.Where(ws => !ws.Item.IsInaccessible).Last().Item;
                    System.Diagnostics.Debug.WriteLine(chosenWorkspace.Id);
                    return chosenWorkspace;
                });
                var observer = TestScheduler.CreateObserver<IThreadSafeWorkspace>();
                ViewModel.SelectWorkspace.Elements.Subscribe(observer);

                await ViewModel.Initialize();
                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(chosenWorkspace);
            }
        }

        public sealed class TheSelectTimeRangeAction : ReportsViewModelTest
        {
            // TODO: Add tests when navigation to the correct viewmodel is implemented.
        }
    }
}
