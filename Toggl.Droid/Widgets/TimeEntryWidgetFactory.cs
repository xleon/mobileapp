using Android.OS;
using System;

namespace Toggl.Droid.Widgets
{
    public static class TimeEntryWidgetFactory
    {
        public static ITimeEntryWidgetFormFactor Create(WidgetDimensions widgetDimensions)
        {
            var dimensions = widgetDimensions ?? WidgetDimensions.Default;

            if (!isLoggedIn)
                return new TimeEntryWidgetNotLoggedInFormFactor(dimensions.ColumnsCount);

            if (dimensions.ColumnsCount == 1)
                return new TimeEntryWidgetButtonOnlyFormFactor();

            return new TimeEntryWidgetDefaultFormFactor();
        }

        private static bool isLoggedIn =>
            AndroidDependencyContainer.Instance.PrivateSharedStorageService.GetApiToken() != null;
    }
}
