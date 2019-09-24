using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Fragments;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using Fragment = Android.Support.V4.App.Fragment;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class MainTabBarActivity : ReactiveActivity<MainTabBarViewModel>
    {
        public static readonly string StartingTabExtra = "StartingTabExtra";
        public static readonly string WorkspaceIdExtra = "WorkspaceIdExtra";
        public static readonly string StartDateExtra = "StartDateExtra";
        public static readonly string EndDateExtra = "EndDateExtra";

        private readonly Dictionary<int, Task<Fragment>> fragments = new Dictionary<int, Task<Fragment>>();
        
        private Fragment activeFragment;
        private bool activityResumedBefore = false;
        private int? requestedInitialTab;
        private long? reportsRequestedWorkspaceId;
        private DateTimeOffset? reportsRequestedStartDate;
        private DateTimeOffset? reportsRequestedEndDate;

        public MainTabBarActivity() : base(
            Resource.Layout.MainTabBarActivity,
            Resource.Style.AppTheme,
            Transitions.Fade)
        { }

        public MainTabBarActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void RestoreViewModelStateFromBundle(Bundle bundle)
        {
            base.RestoreViewModelStateFromBundle(bundle);

            restoreFragmentsViewModels();
            showInitialFragment(getInitialTab(Intent, bundle));
            loadReportsIntentExtras(Intent);
        }

        protected override void InitializeBindings()
        {
            navigationView
                .Rx()
                .ItemSelected()
                .Subscribe(onTabSelected)
                .DisposedBy(DisposeBag);
        }

        private int getInitialTab(Intent intent, Bundle bundle = null)
        {
            var intentTab = intent.GetIntExtra(StartingTabExtra, Resource.Id.MainTabTimerItem);
            if (intentTab != Resource.Id.MainTabTimerItem || bundle == null)
                return intentTab;

            var bundleTab = bundle.GetInt(StartingTabExtra, Resource.Id.MainTabTimerItem);
            return bundleTab;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            requestedInitialTab = getInitialTab(intent);
            loadReportsIntentExtras(intent);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(StartingTabExtra, navigationView.SelectedItemId);
            base.OnSaveInstanceState(outState);
        }

        private void loadReportsIntentExtras(Intent intent)
        {
            var workspaceId = intent.GetLongExtra(WorkspaceIdExtra, 0L);
            var startDate = intent.GetLongExtra(StartDateExtra, 0L);
            var endDate = intent.GetLongExtra(EndDateExtra, 0L);

            if (workspaceId == 0)
                reportsRequestedWorkspaceId = null;

            if (startDate == 0 || endDate == 0)
            {
                reportsRequestedStartDate = default(DateTimeOffset);
                reportsRequestedEndDate = default(DateTimeOffset);
            }
            else
            {
                reportsRequestedStartDate = DateTimeOffset.FromUnixTimeSeconds(startDate);
                reportsRequestedEndDate = DateTimeOffset.FromUnixTimeSeconds(endDate);    
            }
        }

        private void restoreFragmentsViewModels()
        {
            foreach (var frag in SupportFragmentManager.Fragments)
            {
                switch (frag)
                {
                    case MainFragment mainFragment:
                        mainFragment.ViewModel = getTabViewModel<MainViewModel>();
                        break;
                    case ReportsFragment reportsFragment:
                        reportsFragment.ViewModel = getTabViewModel<ReportsViewModel>();
                        break;
                    case CalendarFragment calendarFragment:
                        calendarFragment.ViewModel = getTabViewModel<CalendarViewModel>();
                        break;
                    case SettingsFragment settingsFragment:
                        settingsFragment.ViewModel = getTabViewModel<SettingsViewModel>();
                        break;
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!activityResumedBefore)
            {
                navigationView.SelectedItemId = requestedInitialTab ?? Resource.Id.MainTabTimerItem;
                activityResumedBefore = true;
                requestedInitialTab = null;
                loadReportsIfNeeded();
                return;
            }

            if (requestedInitialTab == null) return;
            navigationView.SelectedItemId = requestedInitialTab.Value;
            requestedInitialTab = null;
            loadReportsIfNeeded();
        }

        private void loadReportsIfNeeded()
        {
            if (reportsRequestedStartDate == null || reportsRequestedEndDate == null)
                return;

            var reportsViewModel = getTabViewModel<ReportsViewModel>();
            if (reportsViewModel != null && navigationView.SelectedItemId == Resource.Id.MainTabReportsItem)
            {
                reportsViewModel.LoadReport(reportsRequestedWorkspaceId, reportsRequestedStartDate.Value, reportsRequestedEndDate.Value, ReportsSource.Other);
            }

            reportsRequestedWorkspaceId = null;
            reportsRequestedStartDate = null;
            reportsRequestedEndDate = null;
        }

        private Task<Fragment> getCachedFragment(int itemId)
        {
            if (fragments.TryGetValue(itemId, out var fragment))
                return fragment;

            fragments[itemId] = Task.Run<Fragment>(async () =>
            {
                await Task.Yield();

                switch (itemId)
                {
                    case Resource.Id.MainTabTimerItem:
                        var mainVM = getTabViewModel<MainViewModel>();
                        await mainVM.Initialize();
                        return new MainFragment { ViewModel = mainVM };
                    case Resource.Id.MainTabReportsItem:
                        var reportsVM = getTabViewModel<ReportsViewModel>();
                        await reportsVM.Initialize();
                        return new ReportsFragment { ViewModel = reportsVM };
                    case Resource.Id.MainTabCalendarItem:
                        var calendarVM = getTabViewModel<CalendarViewModel>();
                        await calendarVM.Initialize();
                        return new CalendarFragment { ViewModel = calendarVM };
                    case Resource.Id.MainTabSettinsItem:
                        var settingsVM = getTabViewModel<SettingsViewModel>();
                        await settingsVM.Initialize();
                        return new SettingsFragment { ViewModel = settingsVM };
                    default:
                        throw new ArgumentException($"Unexpected item id {itemId}");
                }
            });

            return fragments[itemId];
        }

        private TTabViewModel getTabViewModel<TTabViewModel>()
            where TTabViewModel : class, IViewModel
            => ViewModel.GetViewModel<TTabViewModel>();

        public override void OnBackPressed()
        {
            if (navigationView.SelectedItemId == Resource.Id.MainTabTimerItem)
            {
                FinishAfterTransition();
                return;
            }
 
            showFragment(Resource.Id.MainTabTimerItem);

            navigationView.SelectedItemId = Resource.Id.MainTabTimerItem;
        }

        private void onTabSelected(IMenuItem item)
        {
            if (item.ItemId != navigationView.SelectedItemId)
            {
                showFragment(item.ItemId);
                return;
            }

            getCachedFragment(item.ItemId)
                .ContinueWith(t =>
                {
                    var fragment = t.Result;
                    if (fragment is IScrollableToTop scrollableToTop)
                    {
                        scrollableToTop.ScrollToTop();
                    }
                });
        }

        private async Task showFragment(int itemId)
        {
            var fragmentTask = getCachedFragment(itemId);
            tabLoadingIndicator.Visibility = (!fragmentTask.IsCompleted).ToVisibility();
            var fragment = await fragmentTask.ConfigureAwait(false);

            await Task.Run(() =>
            {
                var transaction = SupportFragmentManager.BeginTransaction();

                if (activeFragment is MainFragment mainFragmentToHide)
                    mainFragmentToHide.SetFragmentIsVisible(false);

                if (fragment.IsAdded)
                    transaction.Hide(activeFragment).Show(fragment);
                else
                    transaction.Add(Resource.Id.CurrentTabFragmmentContainer, fragment).Hide(activeFragment);

                transaction.RunOnCommit(new Runnable(() => tabLoadingIndicator.SafeHide()));

                transaction.Commit();

                if (fragment is MainFragment mainFragmentToShow)
                    mainFragmentToShow.SetFragmentIsVisible(true);

                activeFragment = fragment;
            });
        }

        private async Task showInitialFragment(int initialTabItemId)
        {
            SupportFragmentManager.RemoveAllFragments();

            requestedInitialTab = initialTabItemId;
            navigationView.SelectedItemId = initialTabItemId;

            tabLoadingIndicator.Visibility = ViewStates.Visible;
            var initialFragment = await getCachedFragment(initialTabItemId);
            activeFragment = initialFragment;
            if (!initialFragment.IsAdded)
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .Add(Resource.Id.CurrentTabFragmmentContainer, initialFragment)
                    .RunOnCommit(new Runnable(
                        () => tabLoadingIndicator.SafeHide()))
                    .Commit();
            }
            else
            {
                tabLoadingIndicator.Visibility = ViewStates.Gone;
            }

            if (initialFragment is MainFragment mainFragment)
                mainFragment.SetFragmentIsVisible(true);
        }
    }
}
