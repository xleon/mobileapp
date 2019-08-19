using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Activities
{
    public sealed partial class CalendarSettingsActivity
    {
        private View toggleCalendarsView;
        private Switch toggleCalendarsSwitch;
        private View calendarsContainer;
        private RecyclerView calendarsRecyclerView;
        private UserCalendarsRecyclerAdapter userCalendarsAdapter;

        protected override void InitializeViews()
        {
            toggleCalendarsView = FindViewById(Resource.Id.ToggleCalendarsView);
            toggleCalendarsSwitch = FindViewById<Switch>(Resource.Id.ToggleCalendarsSwitch);
            calendarsContainer = FindViewById(Resource.Id.CalendarsContainer);
            calendarsRecyclerView = FindViewById<RecyclerView>(Resource.Id.CalendarsRecyclerView);

            userCalendarsAdapter = new UserCalendarsRecyclerAdapter();
            calendarsRecyclerView.SetAdapter(userCalendarsAdapter);
            calendarsRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
        }
    }
}
