using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        }
        
        public async Task Start()
        {
            revokeNewUserIfNeeded();

            dependencyContainer.BackgroundSyncService.SetupBackgroundSync(dependencyContainer.UserAccessManager);
            
            var timeService = dependencyContainer.TimeService;
            var navigationService = dependencyContainer.NavigationService;
            var onboardingStorage = dependencyContainer.OnboardingStorage;
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                await navigationService.Navigate<OutdatedAppViewModel>();
                return;
            }

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                await navigationService.Navigate<TFirstViewModelWhenNotLoggedIn, TInput>(new TInput());
                return;
            }
            
            var user = await dependencyContainer.InteractorFactory.GetCurrentUser().Execute();
            if (accessRestrictionStorage.IsUnauthorized(user.ApiToken))
            {
                await navigationService.Navigate<TokenResetViewModel>();
                return;
            }

            dependencyContainer.SyncManager.ForceFullSync().Subscribe();

            await navigationService.Navigate<MainTabBarViewModel>();
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
