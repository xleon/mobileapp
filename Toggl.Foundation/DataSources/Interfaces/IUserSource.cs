using System;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IUserSource
    {
        bool IsLoggedIn { get; }

        IObservable<IUser> Login(string username, string password);
    }
}