using System;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IUserSource
    {
        IObservable<IUser> Login(Email username, string password);
    }
}