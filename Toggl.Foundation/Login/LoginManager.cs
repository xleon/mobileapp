using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.Login
{
    public interface ILoginManager
    {
        IObservable<ITogglDataSource> Login(Email email, string password);
    }

    public class LoginManager : ILoginManager
    {
        public delegate ITogglDatabase DatabaseFactory();

        private readonly IApiFactory apiFactory;
        private readonly DatabaseFactory databaseFactory;

        public LoginManager(IApiFactory apiFactory, DatabaseFactory databaseFactory)
        {
            Ensure.Argument.IsNotNull(apiFactory, nameof(apiFactory));
            Ensure.Argument.IsNotNull(databaseFactory, nameof(databaseFactory));

            this.apiFactory = apiFactory;
            this.databaseFactory = databaseFactory;
        }

        public IObservable<ITogglDataSource> Login(Email email, string password)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to Login");
            Ensure.Argument.IsNotNullOrWhiteSpaceString(password, nameof(password));

            var database = databaseFactory();
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
    }
}
