using Android.Widget;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Views;
using static Toggl.Giskard.Resource.Id;

namespace Toggl.Giskard.Activities
{
    public partial class StartTimeEntryActivity
    {
        private TextView durationLabel;

        private AutocompleteEditText editText;

        private void initializeViews()
        {
            durationLabel = FindViewById<TextView>(StartTimeEntryDurationText);

            editText = FindViewById<AutocompleteEditText>(StartTimeEntryDescriptionTextField);
        }
    }
}
