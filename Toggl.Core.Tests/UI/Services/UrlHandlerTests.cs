using System;
using FluentAssertions;
using Microsoft.FSharp.Core;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.Services
{
    public sealed class UrlHandlerTests
    {
        public abstract class UrlHandlerTest : BaseTest
        {
            protected UrlHandler UrlHandler { get; }

            public UrlHandlerTest()
            {
                UrlHandler = new UrlHandler(TimeService, InteractorFactory, NavigationService, ViewPresenter);
            }
        }

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useTimeService, bool useInteractorFactory, bool useNavigationService, bool useViewPresenter)
            {
                var timeService = useTimeService ? Substitute.For<ITimeService>() : null;
                var interactorFactory = useInteractorFactory ? Substitute.For<IInteractorFactory>() : null;
                var navigationService = useNavigationService ? Substitute.For<INavigationService>() : null;
                var viewPresenter = useViewPresenter ? Substitute.For<IPresenter>() : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new UrlHandler(timeService, interactorFactory, navigationService, viewPresenter);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheStartTimeEntryUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesStartTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/start");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received()
                    .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), Arg.Any<TimeEntryStartOrigin>());
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesStartTimeEntryWithParameters()
            {
                var url = new Uri("toggl://tracker/timeEntry/start?description=Hello%20world&startTime=\"2019-05-14T14:30:00Z\"&workspaceId=42&source=Siri");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received()
                    .CreateTimeEntry(
                        Arg.Is<ITimeEntryPrototype>(arg => arg.Description == "Hello world"
                                    && arg.StartTime == new DateTimeOffset(2019, 5, 14, 14, 30, 0, TimeSpan.Zero)
                                    && arg.WorkspaceId == 42),
                        TimeEntryStartOrigin.Siri);
            }
        }

        public sealed class TheContinueTimeEntryUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesContinueTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/continue");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received().ContinueMostRecentTimeEntry();
            }
        }

        public sealed class TheStopTimeEntryUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesStopTimeEntry()
            {
                var now = new DateTimeOffset(2019, 5, 14, 14, 3, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var url = new Uri("toggl://tracker/timeEntry/stop");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received().StopTimeEntry(now, Arg.Any<TimeEntryStopOrigin>());
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesStopTimeEntryWithParameters()
            {
                var url = new Uri("toggl://tracker/timeEntry/stop?stopTime=2019-05-14T14:45:00Z");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received().StopTimeEntry(
                    new DateTimeOffset(2019, 5, 14, 14, 45, 0, TimeSpan.Zero),
                    Arg.Any<TimeEntryStopOrigin>());
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesStopTimeEntryFromSiri()
            {
                var now = new DateTimeOffset(2019, 5, 14, 14, 45, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var url = new Uri("toggl://tracker/timeEntry/stop?stopTime=2019-05-14T14:45:00Z&source=Siri");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received().StopTimeEntry(now, TimeEntryStopOrigin.Siri);
            }
        }

        public sealed class TheNewTimeEntryUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesNewTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/new");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                NavigationService.Received()
                    .Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(Arg.Any<StartTimeEntryParameters>(), null);
            }
        }

        public sealed class TheEditTimeEntryUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesEditTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/edit?timeEntryId=1");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                NavigationService.Received()
                    .Navigate<EditTimeEntryViewModel, long[]>(Arg.Any<long[]>(), null);
            }
        }

        public sealed class TheReportsUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesTheReportsUrl()
            {
                var url = new Uri("toggl://tracker/reports");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                ViewPresenter.Received().ChangePresentation(Arg.Any<ShowReportsPresentationChange>());
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesTheReportsUrlWithParameters()
            {
                var url = new Uri("toggl://tracker/reports?workspaceId=1&startDate=\"2019-05-01T00:00:00Z\"&endDate=\"2019-05-14T00:00:00Z\"");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                ViewPresenter.Received().ChangePresentation(
                    Arg.Is<ShowReportsPresentationChange>(arg =>
                        arg.WorkspaceId == 1
                          && arg.StartDate == new DateTimeOffset(2019, 5, 1, 0, 0, 0, TimeSpan.Zero)
                          && arg.EndDate == new DateTimeOffset(2019, 5, 14, 0, 0, 0, TimeSpan.Zero)));
            }
        }

        public sealed class TheCalendarUrl : UrlHandlerTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesTheCalendarUrl()
            {
                var url = new Uri("toggl://tracker/calendar");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                ViewPresenter.Received().ChangePresentation(Arg.Any<ShowCalendarPresentationChange>());
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesTheCalendarUrlWithParameters()
            {
                var url = new Uri("toggl://tracker/calendar?eventId=1");

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeTrue();
                InteractorFactory.Received()
                    .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), Arg.Any<TimeEntryStartOrigin>());
            }
        }

        public sealed class UnknownUrl : UrlHandlerTest
        {
            [Theory, LogIfTooSlow]
            [InlineData("toggl://tracker/foo")]
            [InlineData("toggl://tracker/foo?source=Siri")]
            [InlineData("toggl://not-toggl/foo?source=Siri")]
            public async Task HandlesTheCalendarUrl(string urlString)
            {
                var url = new Uri(urlString);

                var handled = await UrlHandler.Handle(url);

                handled.Should().BeFalse();
            }
        }
    }
}
