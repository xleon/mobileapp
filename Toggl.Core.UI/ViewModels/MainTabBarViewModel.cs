using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class MainTabBarViewModel : ViewModel
    {
        private readonly IPlatformInfo platformInfo;

        private readonly Lazy<ViewModel> mainViewModel;
        private readonly Lazy<ViewModel> reportsViewModel;
        private readonly Lazy<ViewModel> calendarViewModel;

        public IImmutableList<Lazy<ViewModel>> Tabs { get; }

        public MainTabBarViewModel(UIDependencyContainer dependencyContainer)
            : base(dependencyContainer.NavigationService)
        {
            platformInfo = dependencyContainer.PlatformInfo;

            mainViewModel = new Lazy<ViewModel>(() => new MainViewModel(
                dependencyContainer.DataSource,
                dependencyContainer.SyncManager,
                dependencyContainer.TimeService,
                dependencyContainer.RatingService,
                dependencyContainer.UserPreferences,
                dependencyContainer.AnalyticsService,
                dependencyContainer.OnboardingStorage,
                dependencyContainer.InteractorFactory,
                dependencyContainer.NavigationService,
                dependencyContainer.RemoteConfigService,
                dependencyContainer.AccessibilityService,
                dependencyContainer.UpdateRemoteConfigCacheService,
                dependencyContainer.AccessRestrictionStorage,
                dependencyContainer.SchedulerProvider,
                dependencyContainer.RxActionFactory,
                dependencyContainer.PermissionsChecker,
                dependencyContainer.BackgroundService,
                platformInfo,
                dependencyContainer.WidgetsService,
                dependencyContainer.LastTimeUsageStorage));

            reportsViewModel = new Lazy<ViewModel>(() => new ReportsViewModel(
                dependencyContainer.DataSource,
                dependencyContainer.TimeService,
                dependencyContainer.NavigationService,
                dependencyContainer.InteractorFactory,
                dependencyContainer.AnalyticsService,
                dependencyContainer.SchedulerProvider,
                dependencyContainer.RxActionFactory));

            calendarViewModel = new Lazy<ViewModel>(() => new CalendarViewModel(
                dependencyContainer.DataSource,
                dependencyContainer.TimeService,
                dependencyContainer.RxActionFactory,
                dependencyContainer.UserPreferences,
                dependencyContainer.AnalyticsService,
                dependencyContainer.BackgroundService,
                dependencyContainer.InteractorFactory,
                dependencyContainer.SchedulerProvider,
                dependencyContainer.NavigationService));

            Tabs = getViewModels().ToImmutableList();
        }

        public Lazy<TViewModel> GetViewModel<TViewModel>()
            where TViewModel : class, IViewModel
        {
            var expectedType = typeof(TViewModel);
            if (expectedType == typeof(MainViewModel))
                return mainViewModel.Select(x => x as TViewModel);

            if (expectedType == typeof(ReportsViewModel))
                return reportsViewModel.Select(x => x as TViewModel);

            if (expectedType == typeof(CalendarViewModel))
                return calendarViewModel.Select(x => x as TViewModel);

            throw new InvalidOperationException();
        }

        private IEnumerable<Lazy<ViewModel>> getViewModels()
        {
            yield return mainViewModel;
            yield return reportsViewModel;
            yield return calendarViewModel;
        }
    }
}
