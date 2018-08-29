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

        public DonationReportPeriod DonationPeriod { get; private set; }

        protected CalendarBaseQuickSelectShortcut(
            ITimeService timeService, string title, DonationReportPeriod donationReportPeriod)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(title, nameof(title));
            Ensure.Argument.IsNotNull(donationReportPeriod, nameof(donationReportPeriod));

            TimeService = timeService;
            Title = title;
            DonationPeriod = donationReportPeriod;
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
