using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Extensions;
using Toggl.Droid.Views.Calendar;

namespace Toggl.Droid.Fragments.Calendar
{
    public partial class CalendarDayViewPageFragment
    {
        private ConstraintLayout rootView;
        private CalendarDayView calendarDayView;
        private View contextualMenuContainer;
        private View dismissButton;
        private TextView periodText;
        private TextView timeEntryDetails;
        private RecyclerView actionsRecyclerView;

        private ViewStub calendarDayViewStub;
        private ViewStub contextualMenuViewStub;

        private void initializeViews(View view)
        { 
            rootView = view.FindViewById<ConstraintLayout>(Resource.Id.CalendarDayPage);
            calendarDayViewStub = view.FindViewById<ViewStub>(Resource.Id.CalendarDayViewStub);
            contextualMenuViewStub = view.FindViewById<ViewStub>(Resource.Id.ContextualMenuStub);
        }

        private void initializeCalendarDayView()
        {
            if (calendarDayView != null) return;

            calendarDayView = (CalendarDayView)calendarDayViewStub.Inflate();
            
            var constraintSet = new ConstraintSet();
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Left, ConstraintSet.ParentId, ConstraintSet.Left);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Right, ConstraintSet.ParentId, ConstraintSet.Right);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Bottom, ConstraintSet.ParentId, ConstraintSet.Bottom);
            constraintSet.ApplyTo(rootView);
        }

        private void initializeContextualMenuView()
        {
            if (contextualMenuContainer != null) return;

            contextualMenuContainer = (ConstraintLayout)contextualMenuViewStub.Inflate();
            
            var constraintSet = new ConstraintSet();
            constraintSet.Clone(rootView);
            
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Left, ConstraintSet.ParentId, ConstraintSet.Left);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Right, ConstraintSet.ParentId, ConstraintSet.Right);
            constraintSet.Connect(calendarDayView.Id, ConstraintSet.Bottom, contextualMenuContainer.Id, ConstraintSet.Top);

            constraintSet.Connect(contextualMenuContainer.Id, ConstraintSet.Bottom, ConstraintSet.ParentId, ConstraintSet.Bottom);
            constraintSet.Connect(contextualMenuContainer.Id, ConstraintSet.Left, ConstraintSet.ParentId, ConstraintSet.Left);
            constraintSet.Connect(contextualMenuContainer.Id, ConstraintSet.Right, ConstraintSet.ParentId, ConstraintSet.Right);
            constraintSet.Connect(contextualMenuContainer.Id, ConstraintSet.Top, calendarDayView.Id, ConstraintSet.Bottom);
            constraintSet.ApplyTo(rootView);
            
            contextualMenuContainer.DoOnApplyWindowInsets((v, insets, initialPadding) =>
                contextualMenuContainer.SetPadding(0, 0, 0, insets.SystemWindowInsetBottom));
            
            dismissButton = contextualMenuContainer.FindViewById<ImageView>(Resource.Id.DismissButton); 
            periodText = contextualMenuContainer.FindViewById<TextView>(Resource.Id.PeriodText); 
            timeEntryDetails = contextualMenuContainer.FindViewById<TextView>(Resource.Id.TimeEntryDetails); 
            actionsRecyclerView = contextualMenuContainer.FindViewById<RecyclerView>(Resource.Id.ActionsRecyclerView);
            
            menuActionsAdapter = new CalendarContextualMenuActionsAdapter();
            actionsRecyclerView.SetLayoutManager(new LinearLayoutManager(contextualMenuContainer.Context, LinearLayoutManager.Horizontal, false));
            actionsRecyclerView.SetAdapter(menuActionsAdapter);
        }
    }
}
