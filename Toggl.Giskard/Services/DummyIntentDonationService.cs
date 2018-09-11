using Toggl.Foundation.Services;

namespace Toggl.Giskard.Services
{
    public class DummyIntentDonationService: IIntentDonationService
    {
        public void DonateStartTimeEntry()
        {
            throw new NotImplementedException();
        }

        public void DonateStopCurrentTimeEntry()
        {
            throw new System.NotImplementedException();
        }

        public void DonateShowReport(ReportPeriod period)
        {
            throw new System.NotImplementedException();
        }

        public void DonateShowReport()
        {
            throw new System.NotImplementedException();
        }

        public void ClearAll()
        {
            throw new System.NotImplementedException();
        }
    }
}