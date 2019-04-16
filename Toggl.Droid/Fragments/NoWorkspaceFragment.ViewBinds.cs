using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Toggl.Droid.Fragments
{
    [MvxDialogFragmentPresentation(Cancelable = false)]
    public partial class NoWorkspaceFragment
    {
        private ProgressBar progressBar;
        private TextView tryAgainTextView;
        private TextView createWorkspaceTextView;

        public void InitializeViews(View rootView)
        {
            progressBar = rootView.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            tryAgainTextView = rootView.FindViewById<TextView>(Resource.Id.TryAgainTextView);
            createWorkspaceTextView = rootView.FindViewById<TextView>(Resource.Id.CreateWorkspaceTextView);
        }
    }
}
