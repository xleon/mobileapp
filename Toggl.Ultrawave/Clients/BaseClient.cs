using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Toggl.Multivac;
using System.Reactive.Linq;
using Toggl.Ultrawave.Exceptions;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Clients
{
    internal abstract class BaseClient
    {
        private readonly IApiClient apiClient;
        private readonly IJsonSerializer serializer;

        protected HttpHeader AuthHeader { get; }

        protected BaseClient(IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
        {
            Ensure.ArgumentIsNotNull(apiClient, nameof(apiClient));
            Ensure.ArgumentIsNotNull(serializer, nameof(serializer));
            Ensure.ArgumentIsNotNull(credentials, nameof(credentials));

            this.apiClient = apiClient;
            this.serializer = serializer;
            this.AuthHeader = credentials.Header;
        }

        protected IObservable<T> CreateObservable<T>(Endpoint endpoint, HttpHeader header, T entity, SerializationReason serializationReason) {
            var body = serializer.Serialize<T>(entity, serializationReason);
            return CreateObservable<T>(endpoint, header, body);
        }
        
        protected IObservable<T> CreateObservable<T>(Endpoint endpoint, HttpHeader header, string body = "")
            => CreateObservable<T>(endpoint, new [] { header }, body);

        protected IObservable<T> CreateObservable<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers, string body = "")
        {
            var request = new Request(body, endpoint.Url, headers, endpoint.Method);
            return Observable.Create<T>(async observer =>
            {
                var response = await apiClient.Send(request).ConfigureAwait(false);
                if (response.IsSuccess)
                {
                    var data = await Task.Run(() => serializer.Deserialize<T>(response.RawData)).ConfigureAwait(false);
                    observer.OnNext(data);
                    observer.OnCompleted();
                }
                else
                {
                    //TODO: Treat different error responses here. We need to check those as we create our clients.
                    observer.OnError(new ApiException(response.RawData));
                }
            });
        }
    }
}
