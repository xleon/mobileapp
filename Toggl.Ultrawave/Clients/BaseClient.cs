﻿﻿﻿using System;
using System.Collections.Generic;
using System.Text;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Toggl.Multivac;
using System.Reactive.Linq;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Ultrawave.Clients
{
    internal abstract class BaseClient
    {
        private readonly IApiClient apiClient;
        private readonly IJsonSerializer serializer;

        protected BaseClient(IApiClient apiClient, IJsonSerializer serializer)
        {
            Ensure.ArgumentIsNotNull(apiClient, nameof(apiClient));
            Ensure.ArgumentIsNotNull(serializer, nameof(serializer));

            this.apiClient = apiClient;
            this.serializer = serializer;
        }

        protected HttpHeader GetAuthHeader(string username, string password)
        {
            var authString = $"{username}:{password}";
            var authStringBytes = Encoding.UTF8.GetBytes(authString);
            var authHeader = Convert.ToBase64String(authStringBytes);
                                    
            return new HttpHeader("Authorization", authHeader, HttpHeader.HeaderType.Auth);
        }

        protected IObservable<T> CreateObservable<T>(Endpoint endpoint, HttpHeader header, string body = "")
            => CreateObservable<T>(endpoint, new [] { header }, body);

        protected IObservable<T> CreateObservable<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers, string body = "")
        {
            var request = new Request(body, endpoint.Url, headers, endpoint.Method);
            var observable = Observable.Create<T>(async observer =>
            {
                var response = await apiClient.Send(request).ConfigureAwait(false);
                if (response.IsSuccess)
                {
                    var data = await serializer.Deserialize<T>(response.RawData).ConfigureAwait(false);
                    observer.OnNext(data);
                }
                else
                {
                    //TODO: Treat different error responses here. We need to check those as we create our clients.
                    observer.OnError(new ApiException(response.RawData));
                }

                observer.OnCompleted();
            });

            return observable;
        }
    }
}
