using Android.Appwidget;
using Android.Content;
using Android.OS;
using static Toggl.Droid.Widgets.TimerWidgetFormFactor;

namespace Toggl.Droid.Widgets
{
    internal struct WidgetContext
    {
        private static int defaultMinWidth = 144;
        private static int defaultMinHeight = 72;

        public TimerWidgetFormFactor FormFactor { get; private set; }
        public int ColumnsCount { get; private set; }

        public WidgetContext(TimerWidgetFormFactor formFactor, int columnsCount)
        {
            FormFactor = formFactor;
            ColumnsCount = columnsCount;
        }

        public static WidgetContext From(Bundle bundle)
        {
            var minWidth = bundle.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
            var minHeight = bundle.GetInt(AppWidgetManager.OptionAppwidgetMinHeight);

            var formFactor = determineFormFactor(minWidth, minHeight);

            return new WidgetContext(formFactor, getColumnsCount(minWidth));
        }

        private static TimerWidgetFormFactor determineFormFactor(int minWidth, int minHeight)
        {
            return minHeight < defaultMinHeight || minWidth < defaultMinWidth
                ? ButtonOnly
                : FullWidget;
        }

        /// <summary>
        /// Calculates the number of columns used by the widget based on the given width
        /// </summary>
        /// <remarks>
        /// Magic numbers in this method come from https://developer.android.com/guide/practices/ui_guidelines/widget_design.html#anatomy_determining_size
        /// </remarks>
        private static int getColumnsCount(int width) => (width + 30) / 70;
    }
}
