using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    internal sealed class Request : IRequest
    {
        public Either<string, byte[]> Body { get; }

        public Uri Endpoint { get; }

        public HttpMethod HttpMethod { get; }

        public IEnumerable<HttpHeader> Headers { get; }

        public Request(string body, Uri endpoint, IEnumerable<HttpHeader> headers, HttpMethod httpMethod)
        {
            Ensure.ArgumentIsNotNull(body, nameof(body));
            Ensure.ArgumentIsNotNull(headers, nameof(headers));
            Ensure.ArgumentIsNotNull(endpoint, nameof(endpoint));

            Body = Either<string, byte[]>.Left(body);
            Headers = headers.ToList();
            Endpoint = endpoint;
            HttpMethod = httpMethod;
        }
    }
}
