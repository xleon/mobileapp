using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Multivac;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal abstract class BaseApi
    {
        private readonly IApiClient apiClient;
        private readonly IJsonSerializer serializer;

        protected HttpHeader AuthHeader { get; }

        protected BaseApi(IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
        {
            Ensure.Argument.IsNotNull(apiClient, nameof(apiClient));
            Ensure.Argument.IsNotNull(serializer, nameof(serializer));
            Ensure.Argument.IsNotNull(credentials, nameof(credentials));

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
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                        observer.OnError(new NotAuthorizedException(response.RawData));
                    else
                        observer.OnError(new ApiException(response.RawData));
                }
            });
        }
    }
}
