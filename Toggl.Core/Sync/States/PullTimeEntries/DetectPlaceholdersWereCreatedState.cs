using System;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Sync.States.PullTimeEntries
{
    public sealed class DetectPlaceholdersWereCreatedState : ISyncState
    {
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ITimeService timeService;
        private readonly IInteractorFactory interactorFactory;

        public StateResult Done { get; } = new StateResult();

        public DetectPlaceholdersWereCreatedState(
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
        }

        public IObservable<ITransition> Start()
            => interactorFactory.ContainsPlaceholders()
                .Execute()
                .Select(containsPlaceholders =>
                {
                    if (containsPlaceholders)
                    {
                        lastTimeUsageStorage.SetPlaceholdersWereCreated(timeService.CurrentDateTime);
                    }

                    return Done.Transition();
                });
    }
}
