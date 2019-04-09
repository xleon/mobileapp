using System;
using System.Collections.Generic;
using System.Net.Http;
using Toggl.Shared;

namespace Toggl.Ultrawave.Network
{
    internal interface IRequest
    {
        Either<string, byte[]> Body { get; }

        Uri Endpoint { get; }

        HttpMethod HttpMethod { get; }

        IEnumerable<HttpHeader> Headers { get; }
    }
}
