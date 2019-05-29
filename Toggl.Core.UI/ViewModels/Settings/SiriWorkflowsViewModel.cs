using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Newtonsoft.Json;
using Toggl.Core.Models;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Settings
{
    public class SiriWorkflowsViewModel : ViewModel
    {
        #if USE_PRODUCTION_API
            private const string baseURL = "https://toggl-mobile.firebaseapp.com/";
        #elif DEBUG
            private const string baseURL = "https://toggl-mobile.firebaseapp.com/dev/";
        #else
            private const string baseURL = "https://toggl-mobile.firebaseapp.com/adhoc/";
        #endif

        public IObservable<IEnumerable<SiriWorkflow>> Workflows { get; }

        public SiriWorkflowsViewModel(ISchedulerProvider schedulerProvider)
        {
            Workflows = downloadJson()
                .Select(JsonConvert.DeserializeObject<List<SiriWorkflow>>)
                .AsDriver(schedulerProvider);
        }

        private IObservable<string> downloadJson()
        {
            var url = $"{baseURL}workflows.json";
            return new WebClient().DownloadStringTaskAsync(url).ToObservable();
        }

        public string PathForWorkflow(SiriWorkflow workflow)
        {
            return $"shortcuts://import-workflow?url={baseURL}{workflow.FileName}&name={workflow.Title}";
        }
    }
}
