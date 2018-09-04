using Android.Support.Design.Widget;
using Android.Widget;
using FoundationResources = Toggl.Foundation.Resources;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using Android.Support.V4.Widget;

namespace Toggl.Giskard.Activities
{
    public sealed partial class MainActivity
    {
        private View runningEntryCardFrame;
        private FloatingActionButton playButton;
        private FloatingActionButton stopButton;
        private CoordinatorLayout coordinatorLayout;
        private ImageView toolbarReportsImageView;
        private View mainContentArea;
        private TextView mainRunningTimeEntryTimerLabel;
        private TextView mainRunningTimeEntryDescription;
        private TextView mainRunningTimeEntryAddDescriptionLabel;
        private View mainRunningTimeEntryProjectDotContainer;
        private View mainRunningTimeEntryProjectDotView;
        private TextView mainRunningTimeEntryProjectLabel;
        private TextView mainRunningTimeEntryClientLabel;
        private SwipeRefreshLayout refreshLayout;

        private RecyclerView mainRecyclerView;

        protected void InitializeViews()
        {
            mainRecyclerView = FindViewById<RecyclerView>(Resource.Id.MainRecyclerView);
            runningEntryCardFrame = FindViewById(Resource.Id.MainRunningTimeEntryFrame);
            playButton = FindViewById<FloatingActionButton>(Resource.Id.MainPlayButton);
            stopButton = FindViewById<FloatingActionButton>(Resource.Id.MainStopButton);
            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.MainCoordinatorLayout);
            toolbarReportsImageView = FindViewById<ImageView>(Resource.Id.ToolbarReportsImageView);
            mainContentArea = FindViewById(Resource.Id.MainContentArea);
            mainRunningTimeEntryTimerLabel = FindViewById<TextView>(Resource.Id.MainRunningTimeEntryTimerLabel);
            mainRunningTimeEntryDescription = FindViewById<TextView>(Resource.Id.MainRunningTimeEntryDescription);
            mainRunningTimeEntryAddDescriptionLabel = FindViewById<TextView>(Resource.Id.MainRunningTimeEntryAddDescriptionLabel);
            mainRunningTimeEntryProjectDotContainer = FindViewById(Resource.Id.MainRunningTimeEntryProjectDotContainer);
            mainRunningTimeEntryProjectDotView = FindViewById(Resource.Id.MainRunningTimeEntryProjectDotView);
            mainRunningTimeEntryProjectLabel = FindViewById<TextView>(Resource.Id.MainRunningTimeEntryProjectLabel);
            mainRunningTimeEntryClientLabel = FindViewById<TextView>(Resource.Id.MainRunningTimeEntryClientLabel);
            refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.MainSwipeRefreshLayout);
        }
    }
}
