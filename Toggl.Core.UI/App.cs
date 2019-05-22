using System;
using System.Reactive;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Core.UI
{
    public sealed class App<TFirstViewModelWhenNotLoggedIn, TInput>
        where TFirstViewModelWhenNotLoggedIn : ViewModel<TInput, Unit>
        where TInput : new()
    {
        private readonly UIDependencyContainer dependencyContainer;

        public App(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;

            var timeService = dependencyContainer.TimeService;
            var onboardingStorage = dependencyContainer.OnboardingStorage;
            var backgroundService = dependencyContainer.BackgroundSyncService;

            revokeNewUserIfNeeded();
            backgroundService.SetupBackgroundSync(dependencyContainer.UserAccessManager);

            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);
        }

        public bool NavigateIfUserDoesNotHaveFullAccess()
        {
            var navigationService = dependencyContainer.NavigationService;
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                navigationService.Navigate<OutdatedAppViewModel>(null);
                return false;
            }

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                navigationService.Navigate<TFirstViewModelWhenNotLoggedIn, TInput>(new TInput(), null);
                return false;
            }

            var apiToken = dependencyContainer.UserAccessManager.GetSavedApiToken();
            if (accessRestrictionStorage.IsUnauthorized(apiToken))
            {
                navigationService.Navigate<TokenResetViewModel>(null);
                return false;
            }

            dependencyContainer.SyncManager.ForceFullSync().Subscribe();

            return true;
        }

        private void revokeNewUserIfNeeded()
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
    }
}