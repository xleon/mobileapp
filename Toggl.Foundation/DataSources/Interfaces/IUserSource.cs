using System;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IUserSource
    {
        IObservable<IDatabaseUser> Current();
    }
}