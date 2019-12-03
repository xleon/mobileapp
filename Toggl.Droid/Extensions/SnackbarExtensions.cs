using Android.Widget;
using Google.Android.Material.Snackbar;

namespace Toggl.Droid.Extensions
{
    public static class SnackbarExtensions
    {
        public static void ShowWithoutBottomInsetPadding(this Snackbar snackbar)
        {
            var snackbarView = (FrameLayout) snackbar.View;
            snackbarView.DoOnApplyWindowInsets((v, insets, padding) =>
            {
                snackbarView.UpdatePadding(bottom: padding.Bottom - insets.SystemWindowInsetBottom);
            });
            snackbar.Show();
        }
    }
}
