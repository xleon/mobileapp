using System;
using System.Collections.Generic;

namespace Toggl.Networking.ApiClients
{
    public interface ITimeZonesApi
    {
        IObservable<List<string>> GetAll();
    }
}