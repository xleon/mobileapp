using System;
using Toggl.PrimeRadiant.Onboarding;

namespace Toggl.PrimeRadiant.Settings
{
    public interface IOnboardingStorage
    {
        IObservable<bool> IsNewUser { get; }
        IObservable<bool> StartButtonWasTappedBefore { get; }
        IObservable<bool> ProjectOrTagWasAddedBefore { get; }
        IObservable<bool> StopButtonWasTappedBefore { get; }

        void SetCompletedOnboarding();
        void SetIsNewUser(bool isNewUser);
        void SetLastOpened(DateTimeOffset dateString);

        string GetLastOpened();
        bool CompletedOnboarding();

        void StartButtonWasTapped();
        void ProjectOrTagWasAdded();
        void StopButtonWasTapped();

        bool WasDismissed(IDismissable dismissable);
        void Dismiss(IDismissable dismissable);

        void Reset();
    }
}
