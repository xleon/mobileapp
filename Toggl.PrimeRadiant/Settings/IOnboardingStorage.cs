using System;

namespace Toggl.PrimeRadiant.Settings
{
    public interface IOnboardingStorage
    {
        void SetIsNewUser(bool isNewUser);
        void SetLastOpened(DateTimeOffset dateString);
        void SetCompletedOnboarding();

        bool IsNewUser();
        string GetLastOpened();
        bool CompletedOnboarding();
    }
}
