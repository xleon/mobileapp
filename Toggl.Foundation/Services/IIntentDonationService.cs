namespace Toggl.Foundation.Services
{
    public enum DonationReportPeriod
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
        void DonateStopCurrentTimeEntry();
        void DonateShowReport(DonationReportPeriod period);
        void DonateShowReport();
    }
}