using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntriesViewModel : BaseViewModel
    {
        public ObservableCollection<ITimeEntry> TimeEntries { get; }
            = new ObservableCollection<ITimeEntry>();

        public async override Task Initialize()
        {
            await base.Initialize();
            DataSource.TimeEntries.GetAll().Subscribe(timeEntries => timeEntries.ForEach(TimeEntries.Add));
        }
    }
}
