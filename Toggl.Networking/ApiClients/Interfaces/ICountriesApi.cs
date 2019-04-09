using System;
using System.Collections.Generic;
using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ICountriesApi
    {
        IObservable<List<ICountry>> GetAll();
    }
}
