using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public sealed partial class UpcomingEventsNotificationSettingsFragment
    {
        private TextView setSmartRemindersTitle;
        private TextView setSmartRemindersMessage;
        private RecyclerView recyclerView;

        protected override void InitializeViews(View view)
        {
            setSmartRemindersTitle = view.FindViewById<TextView>(Resource.Id.SetSmartRemindersTitle);
            setSmartRemindersMessage = view.FindViewById<TextView>(Resource.Id.SetSmartRemindersMessage);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.CalendarsRecyclerView);
        }
    }
}
