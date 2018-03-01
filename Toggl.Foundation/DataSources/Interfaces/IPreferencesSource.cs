using System;
using Toggl.Foundation.DTOs;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IPreferencesSource
    {
        IObservable<IDatabasePreferences> Get();

        IObservable<IDatabasePreferences> Update(EditPreferencesDTO dto);
    }
}
