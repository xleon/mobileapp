using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Toggl.Droid.Activities
{
    public partial class SendFeedbackActivity
    {
        private View errorCard;
        private View errorIcon;
        private TextView oopsTextView;
        private TextView errorInfoText;
        private EditText feedbackEditText;
        private Toolbar toolbar;
        private ProgressBar progressBar;

        protected override void InitializeViews()
        {
            errorCard = FindViewById<View>(Resource.Id.ErrorCard);
            errorIcon = FindViewById<View>(Resource.Id.ErrorIcon);
            oopsTextView = FindViewById<TextView>(Resource.Id.OopsTextView);
            errorInfoText = FindViewById<TextView>(Resource.Id.ErrorInfoText);
            feedbackEditText = FindViewById<EditText>(Resource.Id.FeedbackEditText);
            progressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
        }
    }
}
