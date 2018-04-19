using System;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Onboarding;

namespace Toggl.Daneel.Onboarding.MainView
{
    public sealed class SwipeRightOnboardingStep : IOnboardingStep
    {
        private const int minimumTimeEntriesCount = 5;

        public IObservable<bool> ShouldBeVisible { get; }

        public SwipeRightOnboardingStep(IObservable<int> timeEntriesCountObservable)
        {
            Ensure.Argument.IsNotNull(timeEntriesCountObservable, nameof(timeEntriesCountObservable));

            ShouldBeVisible = timeEntriesCountObservable
                .Select(timeEntriesCount => timeEntriesCount >= minimumTimeEntriesCount);
        }
    }
}
