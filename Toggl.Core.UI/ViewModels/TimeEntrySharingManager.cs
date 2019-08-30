using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    public sealed class TimeEntrySharingManager
    {
        private readonly int port = 50000;
        public const int Timeout = 10000;

        private BehaviorSubject<bool> isReceivingSubject = new BehaviorSubject<bool>(false);
        private UdpClient udp;

        public TimeEntrySharingManager(IRxActionFactory factory, ISchedulerProvider schedulerProvider)
        {
            IsReceiving = isReceivingSubject
                .DistinctUntilChanged()
                .AsDriver(false, schedulerProvider);

            EnableSharedPayload = factory.FromAsync(enableSharedPayload);
        }

        public void Stop()
        {
            udp?.Close();
        }

        private async Task<SharePayload> enableSharedPayload()
        {
            if (udp != null)
            {
                udp.Close();
                udp = null;
            }

            try
            {
                isReceivingSubject.OnNext(true);

                udp = new UdpClient(port);
                var cancellation = Task.Delay(Timeout).ContinueWith(task => udp.Close());
                var result = await udp.ReceiveAsync();

                var json = Encoding.UTF8.GetString(result.Buffer);
                var payload = JsonConvert.DeserializeObject<SharePayload>(json);

                return payload;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                isReceivingSubject.OnNext(false);
            }

            return null;
        }

        public OutputAction<SharePayload> EnableSharedPayload { get; private set; }
        public IObservable<bool> IsReceiving { get; private set; }
    }
}
