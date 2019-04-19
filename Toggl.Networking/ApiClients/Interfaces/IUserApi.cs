using System;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IUserApi
        : IUpdatingApiClient<IUser>,
          IPullingSingleApiClient<IUser>
    {
        IObservable<IUser> GetWithGoogle();
        IObservable<string> ResetPassword(Email email);
        IObservable<IUser> SignUp(Email email, Password password, bool termsAccepted, int countryId, string timezone);
        IObservable<IUser> SignUpWithGoogle(string googleToken, bool termsAccepted, int countryId, string timezone);
    }
}
