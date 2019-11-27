﻿using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Toggl.Droid.Fragments
{
    public sealed partial class MainFragment
    {
        protected override int LayoutId => Resource.Layout.MainFragment;

        protected override View LoadingPlaceholderView { get; set; }

        private View runningEntryCardFrame;
        private FloatingActionButton playButton;
        private FloatingActionButton stopButton;
        private CoordinatorLayout coordinatorLayout;
        private TextView timeEntryCardTimerLabel;
        private TextView timeEntryCardDescriptionLabel;
        private TextView timeEntryCardAddDescriptionLabel;
        private View timeEntryCardDotContainer;
        private View timeEntryCardDotView;
        private TextView timeEntryCardProjectClientTaskLabel;
        private SwipeRefreshLayout refreshLayout;
        private RecyclerView mainRecyclerView;
        private ViewStub emptyStateViewStub;
        private View emptyStateView;
        private ViewStub welcomeBackStub;
        private View welcomeBackView;
        private TextView welcomeBackTitle;
        private TextView welcomeBackSubText;
        private Toolbar toolbar;
        private AppBarLayout appBarLayout;

        protected override void InitializeViews(View fragmentView)
        {
            mainRecyclerView = fragmentView.FindViewById<RecyclerView>(Resource.Id.MainRecyclerView);
            runningEntryCardFrame = fragmentView.FindViewById(Resource.Id.MainRunningTimeEntrySheet);
            playButton = fragmentView.FindViewById<FloatingActionButton>(Resource.Id.MainPlayButton);
            stopButton = fragmentView.FindViewById<FloatingActionButton>(Resource.Id.MainStopButton);
            coordinatorLayout = fragmentView.FindViewById<CoordinatorLayout>(Resource.Id.MainCoordinatorLayout);
            timeEntryCardTimerLabel = fragmentView.FindViewById<TextView>(Resource.Id.MainRunningTimeEntryTimerLabel);
            timeEntryCardDescriptionLabel = fragmentView.FindViewById<TextView>(Resource.Id.MainRunningTimeEntryDescription);
            timeEntryCardAddDescriptionLabel = fragmentView.FindViewById<TextView>(Resource.Id.MainRunningTimeEntryAddDescriptionLabel);
            timeEntryCardDotContainer = fragmentView.FindViewById(Resource.Id.MainRunningTimeEntryProjectDotContainer);
            timeEntryCardDotView = fragmentView.FindViewById(Resource.Id.MainRunningTimeEntryProjectDotView);
            timeEntryCardProjectClientTaskLabel = fragmentView.FindViewById<TextView>(Resource.Id.MainRunningTimeEntryProjectClientTaskLabel);
            refreshLayout = fragmentView.FindViewById<SwipeRefreshLayout>(Resource.Id.MainSwipeRefreshLayout);
            emptyStateViewStub = fragmentView.FindViewById<ViewStub>(Resource.Id.EmptyStateViewStub);
            welcomeBackStub = fragmentView.FindViewById<ViewStub>(Resource.Id.WelcomeBackViewStub);
            toolbar = fragmentView.FindViewById<Toolbar>(Resource.Id.Toolbar);
            appBarLayout = fragmentView.FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);

            emptyStateView = emptyStateViewStub.Inflate();
            LoadingPlaceholderView = emptyStateView;

            timeEntryCardAddDescriptionLabel.Text = Shared.Resources.AddDescription;
        }
    }
}
