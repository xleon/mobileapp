using System;
using System.Reactive.Linq;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public sealed class ContinueTimeEntryInteractorTests
    {
        protected IInteractorFactory InteractorFactory = Substitute.For<IInteractorFactory>();

        protected IAnalyticsService AnalyticsService = Substitute.For<IAnalyticsService>();

        protected const long ProjectId = 10;

        protected const long WorkspaceId = 11;

        protected const long UserId = 12;

        protected const long TaskId = 14;

        protected long[] TagIds = { 15 };

        protected const string ValidDescription = "Some random time entry";

        protected DateTimeOffset startTime = new DateTimeOffset(2019, 6, 5, 14, 0, 0, TimeSpan.Zero);

        protected IThreadSafeTimeEntry TimeEntryToContinue => new MockTimeEntry
        {
            Id = 42,
            ProjectId = ProjectId,
            WorkspaceId = WorkspaceId,
            UserId = UserId,
            TaskId = TaskId,
            TagIds = TagIds,
            Description = ValidDescription,
            Start = startTime,
        };

        protected IThreadSafeTimeEntry CreatedTimeEntry => new MockTimeEntry
        {
            Id = 43,
            ProjectId = ProjectId,
            WorkspaceId = WorkspaceId,
            UserId = UserId,
            TaskId = TaskId,
            TagIds = TagIds,
            Description = ValidDescription,
            Start = startTime,
        };

        protected ITimeEntryPrototype TimeEntryPrototype => TimeEntryToContinue.AsTimeEntryPrototype();

        protected const int IndexInLog = 5;

        protected const int DayInLog = 1;

        protected const int DaysInPast = 0;

        [Fact, LogIfTooSlow]
        public void CallsTheCreateTimeEntryInteractor()
        {
            var interactor = new ContinueTimeEntryInteractor(
                InteractorFactory,
                AnalyticsService,
                TimeEntryPrototype,
                ContinueTimeEntryMode.TimeEntriesGroupSwipe,
                IndexInLog,
                DayInLog,
                DaysInPast);

            var _ = interactor.Execute();

            InteractorFactory
                .Received()
                .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), TimeEntryStartOrigin.TimeEntriesGroupSwipe)
                .Execute();
        }

        [Theory, LogIfTooSlow]
        [InlineData(ContinueTimeEntryMode.SingleTimeEntrySwipe, ContinueTimeEntryOrigin.Swipe)]
        [InlineData(ContinueTimeEntryMode.SingleTimeEntryContinueButton, ContinueTimeEntryOrigin.ContinueButton)]
        [InlineData(ContinueTimeEntryMode.TimeEntriesGroupSwipe, ContinueTimeEntryOrigin.GroupSwipe)]
        [InlineData(ContinueTimeEntryMode.TimeEntriesGroupContinueButton, ContinueTimeEntryOrigin.GroupContinueButton)]
        public async Task TracksTheTimeEntryContinuedEvent(ContinueTimeEntryMode continueMode, ContinueTimeEntryOrigin expectedOrigin)
        {
            InteractorFactory
                .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), Arg.Any<TimeEntryStartOrigin>())
                .Execute()
                .Returns(Observable.Return(CreatedTimeEntry));

            var interactor = new ContinueTimeEntryInteractor(
                InteractorFactory,
                AnalyticsService,
                TimeEntryPrototype,
                continueMode,
                IndexInLog,
                DayInLog,
                DaysInPast);

            var _ = await interactor.Execute();

            AnalyticsService
                .Received()
                .TimeEntryContinued
                .Track(expectedOrigin, IndexInLog, DayInLog, DaysInPast);
        }
    }
}
