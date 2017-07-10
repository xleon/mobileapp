using MvvmCross.Platform.UI;

namespace Toggl.Foundation.MvvmCross.Helper
{
    public static class Color
    {
        internal static class Onboarding
        {
            internal static readonly MvxColor TrackPageBorderColor = new MvxColor(14, 150, 213);
            internal static readonly MvxColor TrackPageBackgroundColor = new MvxColor(6, 170, 245);

            internal static readonly MvxColor LogPageBorderColor = new MvxColor(165, 81, 220);
            internal static readonly MvxColor LogPageBackgroundColor = new MvxColor(187, 103, 241);

            internal static readonly MvxColor SummaryPageBorderColor = new MvxColor(230, 179, 31);
            internal static readonly MvxColor SummaryPageBackgroundColor = new MvxColor(241, 195, 63);

            internal static readonly MvxColor LoginPageBackgroundColor = new MvxColor(219, 40, 46);
        }

        public static class NavigationBar
        {
            public static readonly MvxColor BackgroundColor = new MvxColor(250, 251, 252);
        }

        public static class TimeEntriesLog
        {
            public static readonly MvxColor ButtonBorder = new MvxColor(243, 243, 243);
        }
    }
}
