using System;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using Toggl.Ultrawave.Helpers;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class StatusApi : IStatusApi
    {
        private readonly IApiClient apiClient;

        public StatusApi(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IObservable<bool> Get()
        {
            return Observable.Create<bool>(async observer =>
            {
                try
                {
                    var request = new Request("", ApiUrls.StatusUrl, Enumerable.Empty<HttpHeader>(), HttpMethod.Get);
                    var response = await apiClient.Send(request).ConfigureAwait(false);

                    observer.OnNext(response.IsSuccess);
                }
                catch
                {
                    observer.OnNext(false);
                }

                observer.OnCompleted();
            });
        }
    }
}
