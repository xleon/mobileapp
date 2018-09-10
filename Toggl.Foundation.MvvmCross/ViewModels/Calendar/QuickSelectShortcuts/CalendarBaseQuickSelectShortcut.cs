using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using Toggl.Foundation.Services;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts
{
    [Preserve(AllMembers = true)]
    public abstract class CalendarBaseQuickSelectShortcut : MvxNotifyPropertyChanged
    {
        protected ITimeService TimeService { get; private set; }

        public string Title { get; }

        public bool Selected { get; private set; }

        public ReportPeriod Period { get; private set; }

        protected CalendarBaseQuickSelectShortcut(
            ITimeService timeService, string title, ReportPeriod reportPeriod)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(title, nameof(title));
            Ensure.Argument.IsNotNull(reportPeriod, nameof(reportPeriod));

            TimeService = timeService;
            Title = title;
            Period = reportPeriod;
        }

        public void OnDateRangeChanged(ReportsDateRangeParameter dateRange)
        {
            var thisActionDateRange = GetDateRange();

            Selected = dateRange.StartDate.Date == thisActionDateRange.StartDate.Date
                    && dateRange.EndDate.Date == thisActionDateRange.EndDate.Date;
        }

        public abstract ReportsDateRangeParameter GetDateRange();
    }
}
