using System;

namespace Toggl.Core.UI.Services
{
    public interface IPasswordManagerService
    {
        bool IsAvailable { get; }

        IObservable<PasswordManagerResult> GetLoginInformation();
    }
}
