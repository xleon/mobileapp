using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public sealed class ContinueTimeEntryInteractor : IInteractor<IObservable<IThreadSafeTimeEntry>>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ITimeEntryPrototype timeEntryPrototype;
        private readonly ContinueTimeEntryMode continueMode;
        private readonly int indexInLog;
        private readonly int dayInLog;
        private readonly int daysInThePast;

        public ContinueTimeEntryInteractor(
            IInteractorFactory interactorFactory,
            IAnalyticsService analyticsService,
            ITimeEntryPrototype timeEntryPrototype,
            ContinueTimeEntryMode continueMode,
            int indexInLog,
            int dayInLog,
            int daysInThePast)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(timeEntryPrototype, nameof(timeEntryPrototype));
            Ensure.Argument.IsNotNull(continueMode, nameof(continueMode));

            this.interactorFactory = interactorFactory;
            this.analyticsService = analyticsService;
            this.timeEntryPrototype = timeEntryPrototype;
            this.continueMode = continueMode;
            this.indexInLog = indexInLog;
            this.dayInLog = dayInLog;
            this.daysInThePast = daysInThePast;
        }

        public IObservable<IThreadSafeTimeEntry> Execute()
        {
            return interactorFactory
                .CreateTimeEntry(timeEntryPrototype, (TimeEntryStartOrigin)continueMode)
                .Execute()
                .Do(_ => trackContinueEvent());
        }

        private void trackContinueEvent()
        {
            analyticsService.TimeEntryContinued.Track(originFromContinuationMode(continueMode), indexInLog, dayInLog, daysInThePast);
        }

        private ContinueTimeEntryOrigin originFromContinuationMode(ContinueTimeEntryMode mode)
        {
            switch (mode)
            {
                case ContinueTimeEntryMode.SingleTimeEntrySwipe:
                    return ContinueTimeEntryOrigin.Swipe;
                case ContinueTimeEntryMode.SingleTimeEntryContinueButton:
                    return ContinueTimeEntryOrigin.ContinueButton;
                case ContinueTimeEntryMode.TimeEntriesGroupSwipe:
                    return ContinueTimeEntryOrigin.GroupSwipe;
                case ContinueTimeEntryMode.TimeEntriesGroupContinueButton:
                    return ContinueTimeEntryOrigin.GroupContinueButton;
            }

            return ContinueTimeEntryOrigin.Other;
        }
    }
}
