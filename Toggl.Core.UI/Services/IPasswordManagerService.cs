using System;

namespace Toggl.Core.MvvmCross.Services
{
    public interface IPasswordManagerService
    {
        bool IsAvailable { get; }

        IObservable<PasswordManagerResult> GetLoginInformation();
    }
}
