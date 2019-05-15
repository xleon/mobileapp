using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
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
    [Activity(Theme = "@style/AppTheme",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class MainTabBarActivity : ReactiveActivity<MainTabBarViewModel>
    {
        private readonly Dictionary<int, Fragment> fragments = new Dictionary<int, Fragment>();
        private Fragment activeFragment;
        private bool activityResumedBefore = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainTabBarActivity);
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            InitializeViews();

            restoreFragmentsViewModels();
            showInitialFragment();

            navigationView
                .Rx()
                .ItemSelected()
                .Subscribe(onTabSelected)
                .DisposedBy(DisposeBag);
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
                navigationView.SelectedItemId = Resource.Id.MainTabTimerItem;
                activityResumedBefore = true;
            }
        }

        private Fragment getCachedFragment(int itemId)
        {
            if (fragments.TryGetValue(itemId, out var fragment))
                return fragment;

            switch (itemId)
            {
                case Resource.Id.MainTabTimerItem:
                    fragment = new MainFragment { ViewModel = getTabViewModel<MainViewModel>() };
                    break;
                case Resource.Id.MainTabReportsItem:
                    fragment = new ReportsFragment { ViewModel = getTabViewModel<ReportsViewModel>() };
                    break;
                case Resource.Id.MainTabCalendarItem:
                    fragment = new CalendarFragment { ViewModel = getTabViewModel<CalendarViewModel>() };
                    break;
                case Resource.Id.MainTabSettinsItem:
                    fragment = new SettingsFragment { ViewModel = getTabViewModel<SettingsViewModel>() };
                    break;
                default:
                    throw new ArgumentException($"Unexpected item id {itemId}");
            }
            fragments[itemId] = fragment;
            return fragment;
        }

        private TTabViewModel getTabViewModel<TTabViewModel>()
            where TTabViewModel : class, IViewModel
            => ViewModel.Tabs.OfType<TTabViewModel>().Single();

        public override void OnBackPressed()
        {
            if (navigationView.SelectedItemId == Resource.Id.MainTabTimerItem)
            {
                base.OnBackPressed();
                return;
            }

            var fragment = getCachedFragment(Resource.Id.MainTabTimerItem);
            showFragment(fragment);

            navigationView.SelectedItemId = Resource.Id.MainTabTimerItem;
        }

        private void onTabSelected(IMenuItem item)
        {
            var fragment = getCachedFragment(item.ItemId);
            if (item.ItemId != navigationView.SelectedItemId)
            {
                showFragment(fragment);
                return;
            }

            if (fragment is IScrollableToTop scrollableToTop)
            {
                scrollableToTop.ScrollToTop();
            }
        }

        private void showFragment(Fragment fragment)
        {
            var transaction = SupportFragmentManager.BeginTransaction();

            if (activeFragment is MainFragment mainFragmentToHide)
                mainFragmentToHide.SetFragmentIsVisible(false);

            if (fragment.IsAdded)
                transaction.Hide(activeFragment).Show(fragment);
            else
                transaction.Add(Resource.Id.CurrentTabFragmmentContainer, fragment).Hide(activeFragment);

            transaction.Commit();

            if (fragment is MainFragment mainFragmentToShow)
                mainFragmentToShow.SetFragmentIsVisible(true);

            activeFragment = fragment;
        }

        private void showInitialFragment()
        {
            SupportFragmentManager.RemoveAllFragments();

            var mainFragment = getCachedFragment(Resource.Id.MainTabTimerItem) as MainFragment;
            SupportFragmentManager
                .BeginTransaction()
                .Add(Resource.Id.CurrentTabFragmmentContainer, mainFragment)
                .Commit();

            mainFragment.SetFragmentIsVisible(true);

            activeFragment = mainFragment;
        }
    }
}
