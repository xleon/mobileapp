using Toggl.Shared.Models;
using Toggl.Core.Services;

namespace Toggl.Droid.Services
{
    public class NoopIntentDonationServiceAndroid: IIntentDonationService
    {
        public void SetDefaultShortcutSuggestions(IWorkspace workspace)
        {
        }

        public void DonateStartTimeEntry(IWorkspace workspace, ITimeEntry timeEntry)
        {
        }

        public void DonateStopCurrentTimeEntry()
        {
        }

        public void DonateShowReport(ReportPeriod period)
        {
        }

        public void DonateShowReport()
        {
        }

        public void ClearAll()
        {
        }
    }
}