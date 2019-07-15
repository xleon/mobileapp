using System;

namespace Toggl.Core.UI
{
    public sealed class AppStart
    {
        private readonly UIDependencyContainer dependencyContainer;

        public AppStart(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public void SetupBackgroundSync()
        {
            var backgroundService = dependencyContainer.BackgroundSyncService;
            backgroundService.SetupBackgroundSync(dependencyContainer.UserAccessManager);
        }

        public void SetFirstOpened()
        {
            var timeService = dependencyContainer.TimeService;
            var onboardingStorage = dependencyContainer.OnboardingStorage;
            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);
        }

        public AccessLevel GetAccessLevel()
        {
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
                return AccessLevel.AccessRestricted;

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
                return AccessLevel.NotLoggedIn;

            var apiToken = dependencyContainer.UserAccessManager.GetSavedApiToken();
            if (accessRestrictionStorage.IsUnauthorized(apiToken))
                return AccessLevel.TokenRevoked;

            return AccessLevel.LoggedIn;
        }

        public void UpdateOnboardingProgress()
        {
            const int newUserThreshold = 60;
            var now = dependencyContainer.TimeService.CurrentDateTime;
            var lastUsed = dependencyContainer.OnboardingStorage.GetLastOpened();
            dependencyContainer.OnboardingStorage.SetLastOpened(now);
            if (!lastUsed.HasValue)
                return;

            var offset = now - lastUsed;
            if (offset < TimeSpan.FromDays(newUserThreshold))
                return;

            dependencyContainer.OnboardingStorage.SetIsNewUser(false);
        }

        public void ForceFullSync()
        {
            dependencyContainer.SyncManager.ForceFullSync().Subscribe();
        }
    }
}