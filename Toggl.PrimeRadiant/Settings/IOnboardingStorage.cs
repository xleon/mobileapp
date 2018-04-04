using System;
using Toggl.PrimeRadiant.Onboarding;

namespace Toggl.PrimeRadiant.Settings
{
    public interface IOnboardingStorage
    {
        IObservable<bool> IsNewUser { get; }
        IObservable<bool> StartButtonWasTappedBefore { get; }
        IObservable<bool> StopButtonWasTappedBefore { get; }

        void SetIsNewUser(bool isNewUser);
        void SetLastOpened(DateTimeOffset dateString);
        void SetCompletedOnboarding();

        string GetLastOpened();
        bool CompletedOnboarding();

        void StartButtonWasTapped();
        void StopButtonWasTapped();

        bool WasDismissed(IDismissable dismissable);
        void Dismiss(IDismissable dismissable);

        void Reset();
    }
}
