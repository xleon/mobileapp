using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Onboarding;

namespace Toggl.Core.UI.Onboarding.MainView
{
    public sealed class SwipeRightOnboardingStep : IOnboardingStep
    {
        private const int minimumTimeEntriesCount = 5;

        public IObservable<bool> ShouldBeVisible { get; }

        public SwipeRightOnboardingStep(IObservable<bool> conflictingStepsAreNotVisibleObservable, IObservable<int> timeEntriesCountObservable)
        {
            Ensure.Argument.IsNotNull(conflictingStepsAreNotVisibleObservable, nameof(conflictingStepsAreNotVisibleObservable));
            Ensure.Argument.IsNotNull(timeEntriesCountObservable, nameof(timeEntriesCountObservable));

            ShouldBeVisible = Observable.CombineLatest(
                conflictingStepsAreNotVisibleObservable,
                timeEntriesCountObservable,
                (conflictingStepsAreNotVisible, timeEntriesCount) =>
                    conflictingStepsAreNotVisible && timeEntriesCount >= minimumTimeEntriesCount);
        }
    }
}
