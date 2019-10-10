using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.iOS.ExtensionKit.Exceptions;
using Toggl.iOS.ExtensionKit.Models;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Shared.Extensions;
using System.Threading.Tasks;
using Toggl.iOS.ExtensionKit;
using Toggl.iOS.ExtensionKit.Analytics;

namespace Toggl.iOS.TimerWidgetExtension
{
    internal class NetworkingHandler
    {
        private readonly ITogglApi togglApi;

        public NetworkingHandler(ITogglApi togglApi)
        {
           Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));

           this.togglApi = togglApi;
        }

        public async Task StartTimeEntry(TimeEntry timeEntry)
        {
            await togglApi.TimeEntries
                .Create(timeEntry)
                .Do(_ =>
                {
                    SharedStorage.Instance.SetNeedsSync(true);
                    SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.StartTimer());
                },
                exception =>
                {
                    SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.Error(exception.Message));
                })
                .FirstAsync();
        }

        public async Task StopRunningTimeEntry()
        {
            await togglApi.TimeEntries
                .GetAll()
                .Select(getRunningTimeEntry)
                .Select(stopTimeEntry)
                .Do(_ =>
                {
                    SharedStorage.Instance.SetNeedsSync(true);
                    SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.StopTimer());
                },
                exception =>
                {
                    SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.Error(exception.Message));
                })
                .FirstAsync();
        }

        private async Task stopTimeEntry(ITimeEntry timeEntry)
        {
            if (timeEntry == null)
                return;

            var duration = (long)(DateTime.Now - timeEntry.Start).TotalSeconds;
            await togglApi.TimeEntries.Update(
                TimeEntry.from(timeEntry).with(duration)
            );
        }

        private ITimeEntry getRunningTimeEntry(IList<ITimeEntry> timeEntries)
        {
            try
            {
                var runningTE = timeEntries.Where(te => te.Duration == null).First();
                return runningTE;
            }
            catch
            {
                return null;
            }
        }
    }
}
