using Toggl.Foundation.Services;

namespace Toggl.Giskard.Services
{
    public class DummyIntentDonationService: IIntentDonationService
    {
        public Void DonateStartTimeEntry()
        {
            throw new NotImplementedException();
        }

        public void DonateStopCurrentTimeEntry()
        {
            throw new System.NotImplementedException();
        }

        public void DonateShowReport(DonationReportPeriod period)
        {
            throw new System.NotImplementedException();
        }

        public void DonateShowReport()
        {
            throw new System.NotImplementedException();
        }
    }
}