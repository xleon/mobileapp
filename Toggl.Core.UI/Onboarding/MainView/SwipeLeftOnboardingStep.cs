using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Onboarding.MainView
{
    public sealed class SwipeLeftOnboardingStep : IOnboardingStep
    {
        private const int minimumTimeEntriesCount = 5;

        private readonly TimeSpan delay = TimeSpan.FromSeconds(2);

        public IObservable<bool> ShouldBeVisible { get; }

        public SwipeLeftOnboardingStep(
            IObservable<bool> conflictingStepsAreNotVisibleObservable,
            IObservable<int> timeEntriesCountObservable)
        {
            Ensure.Argument.IsNotNull(
                conflictingStepsAreNotVisibleObservable,
                nameof(conflictingStepsAreNotVisibleObservable));
            Ensure.Argument.IsNotNull(timeEntriesCountObservable, nameof(timeEntriesCountObservable));

            ShouldBeVisible = Observable.CombineLatest(
                conflictingStepsAreNotVisibleObservable,
                timeEntriesCountObservable,
                (conflictingStepsAreNotVisible, timeEntriesCount) => conflictingStepsAreNotVisible && timeEntriesCount >= minimumTimeEntriesCount)
                .Throttle(delay);
        }
    }
}
