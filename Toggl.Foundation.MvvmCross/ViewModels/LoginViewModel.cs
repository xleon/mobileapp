using System.Reactive.Linq;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac.Models;
using System;
using PropertyChanged;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Ultrawave.Network;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [ImplementPropertyChanged]
    public class LoginViewModel : BaseViewModel
    {
        private IDisposable loginDisposable;
        private readonly IApiFactory apiFactory;

        public string Email { get; set; }

        public string Password { get; set; }

        public IMvxCommand LoginCommand { get; }

        public LoginViewModel(IApiFactory apiFactory)
        {
            this.apiFactory = apiFactory;
            LoginCommand = new MvxCommand(login, loginCanExecute);
        }

        private void login()
        {
            loginDisposable =
                DataSource.User
                          .Login(Email, Password)
                          .Subscribe(onUser, onError);

            LoginCommand.RaiseCanExecuteChanged();
        }

        private bool loginCanExecute() => loginDisposable == null;

        private void onUser(IUser user)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();

            var credentials = Credentials.WithApiToken(user.ApiToken);
            var api = apiFactory.CreateApiWith(credentials);
            var database = Mvx.Resolve<ITogglDatabase>();
            var dataSource = new TogglDataSource(database, api);

            Mvx.RegisterSingleton(api);
            Mvx.RegisterSingleton<ITogglDataSource>(dataSource);
        }

        private void onError(Exception ex)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}