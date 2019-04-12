using System;
using System.Reactive;

namespace Toggl.Core.Login
{
    public interface IGoogleService
    {
        IObservable<string> GetAuthToken();
        IObservable<Unit> LogOutIfNeeded();
    }
}
