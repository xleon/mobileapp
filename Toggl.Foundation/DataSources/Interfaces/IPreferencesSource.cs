using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IPreferencesSource
    {
        IObservable<IDatabasePreferences> Get();

        IObservable<IDatabasePreferences> Update(IDatabasePreferences preferences);
    }
}
