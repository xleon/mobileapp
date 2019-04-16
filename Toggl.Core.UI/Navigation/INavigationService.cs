using MvvmCross.Navigation;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;

[assembly: MvxNavigation(typeof(MainViewModel), ApplicationUrls.Main.Regex)]
[assembly: MvxNavigation(typeof(ReportsViewModel), ApplicationUrls.Reports)]
[assembly: MvxNavigation(typeof(CalendarViewModel), ApplicationUrls.Calendar.Regex)]
[assembly: MvxNavigation(typeof(StartTimeEntryViewModel), ApplicationUrls.StartTimeEntry)]

namespace Toggl.Core.UI.Navigation
{
    public interface INavigationService : IMvxNavigationService
    {
    }
}
