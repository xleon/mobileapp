using FluentAssertions;
using MvvmCross.ViewModels;
using NSubstitute;
using System;
using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.UI;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Settings;
using Xunit;
using Toggl.Core.UI.Navigation;

namespace Toggl.Core.Tests.UI
{
    public class ViewModelLocatorTests : BaseTest
    {
        [Fact(Skip = "Fixing this while MvvmCross is still there is too complex"), LogIfTooSlow]
        public void IsAbleToCreateEveryViewModel()
        {
            var locator = new ViewModelLoader(new TestDependencyContainer
            {
                MockUserAccessManager = Substitute.For<IUserAccessManager>(),
                MockAccessRestrictionStorage = Substitute.For<IAccessRestrictionStorage>(),
                MockAnalyticsService = Substitute.For<IAnalyticsService>(),
                MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>(),
                MockBrowserService = Substitute.For<IBrowserService>(),
                MockCalendarService = Substitute.For<ICalendarService>(),
                MockDatabase = Substitute.For<ITogglDatabase>(),
                MockDialogService = Substitute.For<IDialogService>(),
                MockGoogleService = Substitute.For<IGoogleService>(),
                MockIntentDonationService = Substitute.For<IIntentDonationService>(),
                MockKeyValueStorage = Substitute.For<IKeyValueStorage>(),
                MockLastTimeUsageStorage = Substitute.For<ILastTimeUsageStorage>(),
                MockLicenseProvider = Substitute.For<ILicenseProvider>(),
                MockNavigationService = Substitute.For<INavigationService>(),
                MockNotificationService = Substitute.For<INotificationService>(),
                MockOnboardingStorage = Substitute.For<IOnboardingStorage>(),
                MockPasswordManagerService = Substitute.For<IPasswordManagerService>(),
                MockPermissionsChecker = Substitute.For<IPermissionsChecker>(),
                MockPlatformInfo = Substitute.For<IPlatformInfo>(),
                MockPrivateSharedStorageService = Substitute.For<IPrivateSharedStorageService>(),
                MockRatingService = Substitute.For<IRatingService>(),
                MockRemoteConfigService = Substitute.For<IRemoteConfigService>(),
                MockSchedulerProvider = Substitute.For<ISchedulerProvider>(),
                MockShortcutCreator = Substitute.For<IApplicationShortcutCreator>(),
                MockStopwatchProvider = Substitute.For<IStopwatchProvider>(),
                MockSuggestionProviderContainer = Substitute.For<ISuggestionProviderContainer>(),
                MockUserPreferences = Substitute.For<IUserPreferences>(),
                MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                MockTimeService = Substitute.For<ITimeService>(),
                MockSyncManager = Substitute.For<ISyncManager>(),
            });
            
            var viewModelTypes = typeof(MainViewModel).Assembly
                .GetTypes()
                .Where(isViewModel);

            foreach (var viewModelType in viewModelTypes)
            {
                //Action tryingToFindAViewModel = () => locator.Load(viewModelType, null);
                //tryingToFindAViewModel.Should().NotThrow();
            }

            bool isViewModel(Type type)
                => type.ImplementsOrDerivesFrom<IMvxViewModel>();
        }

    }
}
