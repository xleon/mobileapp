using System;
using System.Collections.Generic;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface ICountriesApi
    {
        IObservable<List<ICountry>> GetAll();
    }
}
