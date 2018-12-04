using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Foundation.Login;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.Services
{
    public abstract class BaseBackgroundSyncService : IBackgroundSyncService, IDisposable
    {
        private IDisposable loggedInDisposable;
        private IDisposable loggedOutDisposable;

        public void SetupBackgroundSync(LoginManager loginManager)
        {
            loggedInDisposable = loginManager.UserLoggedIn
                .Do(EnableBackgroundSync)
                .Subscribe();

            loggedOutDisposable = loginManager.UserLoggedOut
                .Do(DisableBackgroundSync)
                .Subscribe();
        }

        public abstract void EnableBackgroundSync();
        public abstract void DisableBackgroundSync();

        public void Dispose()
        {
            loggedInDisposable.Dispose();
            loggedOutDisposable.Dispose();
        }
    }
}
