using Android.Widget;
using Google.Android.Material.Snackbar;

namespace Toggl.Droid.Extensions
{
    public static class SnackbarExtensions
    {
        public static void ShowWithoutBottomInsetPadding(this Snackbar snackbar)
        {
            var snackbarView = (FrameLayout) snackbar.View;
            snackbarView.DoOnApplyWindowInsets((v, insets, initialSpacing) =>
            {
                snackbarView.UpdatePadding(bottom: initialSpacing.Padding.Bottom - insets.SystemWindowInsetBottom);
            });
            snackbar.Show();
        }
    }
}
