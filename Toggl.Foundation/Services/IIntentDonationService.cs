namespace Toggl.Foundation.Services
{
    public enum ReportPeriod
    {
        Unknown,
        LastMonth,
        LastWeek,
        Yesterday,
        Today,
        ThisWeek,
        ThisMonth,
        ThisYear
    }

    public interface IIntentDonationService
    {
        void DonateStartTimeEntry();
        void DonateStopCurrentTimeEntry();
        void DonateShowReport(ReportPeriod period);
        void DonateShowReport();
        void ClearAll();
    }
}