using System;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IPasswordManagerService
    {
        bool IsAvailable { get; }

        IObservable<PasswordManagerResult> GetLoginInformation();
    }
}
