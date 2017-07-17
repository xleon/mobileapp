using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.Login
{
    public class LoginManager : ILoginManager
    {
        private readonly IApiFactory apiFactory;
        private readonly ITogglDatabase database;

        public LoginManager(IApiFactory apiFactory, ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(apiFactory, nameof(apiFactory));

            this.database = database;
            this.apiFactory = apiFactory;
        }

        public IObservable<ITogglDataSource> Login(Email email, string password)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to Login");
            Ensure.Argument.IsNotNullOrWhiteSpaceString(password, nameof(password));

            var credentials = Credentials.WithPassword(email, password);

            return database
                    .Clear()
                    .SelectMany(_ => apiFactory.CreateApiWith(credentials).User.Get())
                    .Select(User.Clean)
                    .SelectMany(database.User.Create)
                    .Select(user =>
                    {
                        var newCredentials = Credentials.WithApiToken(user.ApiToken);
                        var api = apiFactory.CreateApiWith(newCredentials);

                        return new TogglDataSource(database, api);
                    });
        }

        public ITogglDataSource GetDataSourceIfLoggedIn()
        {
            return database.User
                       .Single()
                       .Select(dataSourceFromUser)
                       .Catch(Observable.Return<ITogglDataSource>(null))
                       .Wait();
        }

        private ITogglDataSource dataSourceFromUser(IUser user)
        {
            var newCredentials = Credentials.WithApiToken(user.ApiToken);
            var api = apiFactory.CreateApiWith(newCredentials);

            return new TogglDataSource(database, api);
        }
    }
}
