using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public class MainTabViewModelTests
    {
        public abstract class MainTabViewModelTest : BaseViewModelTests<MainTabBarViewModel>
        {
            protected override MainTabBarViewModel CreateViewModel()
                => new MainTabBarViewModel(new TestDependencyContainer
                {
                    MockUserAccessManager = Substitute.For<IUserAccessManager>(),
                    MockAccessRestrictionStorage = Substitute.For<IAccessRestrictionStorage>(),
                    MockAnalyticsService = Substitute.For<IAnalyticsService>(),
                    MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>(),
                    MockCalendarService = Substitute.For<ICalendarService>(),
                    MockDatabase = Substitute.For<ITogglDatabase>(),
                    MockDataSource = Substitute.For<ITogglDataSource>(),
                    MockKeyValueStorage = Substitute.For<IKeyValueStorage>(),
                    MockLastTimeUsageStorage = Substitute.For<ILastTimeUsageStorage>(),
                    MockLicenseProvider = Substitute.For<ILicenseProvider>(),
                    MockNavigationService = Substitute.For<INavigationService>(),
                    MockNotificationService = Substitute.For<INotificationService>(),
                    MockAccessibilityService = Substitute.For<IAccessibilityService>(),
                    MockOnboardingStorage = Substitute.For<IOnboardingStorage>(),
                    MockPermissionsChecker = Substitute.For<IPermissionsChecker>(),
                    MockPlatformInfo = Substitute.For<IPlatformInfo>(),
                    MockPrivateSharedStorageService = Substitute.For<IPrivateSharedStorageService>(),
                    MockRatingService = Substitute.For<IRatingService>(),
                    MockRemoteConfigService = Substitute.For<IRemoteConfigService>(),
                    MockSchedulerProvider = Substitute.For<ISchedulerProvider>(),
                    MockShortcutCreator = Substitute.For<IApplicationShortcutCreator>(),
                    MockUserPreferences = Substitute.For<IUserPreferences>(),
                    MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                    MockTimeService = Substitute.For<ITimeService>(),
                    MockSyncManager = Substitute.For<ISyncManager>(),
                    MockPushNotificationsTokenService = Substitute.For<IPushNotificationsTokenService>(),
                    MockUpdateRemoteConfigCacheService = Substitute.For<IUpdateRemoteConfigCacheService>()
                });
        }
    }
}
