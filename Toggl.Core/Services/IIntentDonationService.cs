using System.Threading.Tasks;
using Toggl.Shared.Models;

namespace Toggl.Core.Services
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
        ThisYear,
        LastYear
    }

    public interface IIntentDonationService
    {
        void SetDefaultShortcutSuggestions(IWorkspace workspace);
        Task DonateStartTimeEntry(IWorkspace workspace, ITimeEntry timeEntry);
        void DonateStopCurrentTimeEntry();
        void DonateShowReport(ReportPeriod period);
        void DonateShowReport();
        void ClearAll();
    }
}