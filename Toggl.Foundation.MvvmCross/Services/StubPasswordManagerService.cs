using System;
namespace Toggl.Foundation.MvvmCross.Services
{
    public class StubPasswordManagerService : IPasswordManagerService
    {
        public bool IsAvailable => false;

        public IObservable<PasswordManagerResult> GetLoginInformation()
            => throw new NotImplementedException("You need to implement IPasswordManagerService and register it on the platform specific setup during the InitializeLastChance method before using it.");
    }
}
