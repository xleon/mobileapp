using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Toggl.Ultrawave.Network
{
    internal interface IRequest
    {
        object Body { get; }

        Uri Endpoint { get; }

        HttpMethod HttpMethod { get; }

        Dictionary<string, string> Headers { get; }
    }
}
