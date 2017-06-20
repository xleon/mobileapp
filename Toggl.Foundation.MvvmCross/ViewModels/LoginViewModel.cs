using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using EmailType = Toggl.Multivac.Email;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class LoginViewModel : BaseViewModel<LoginParameter>
    {
        private IDisposable loginDisposable;

        private EmailType userEmail = EmailType.Invalid;
        private readonly IApiFactory apiFactory;

        private string email = "";
        public string Email 
        {     
            get { return email; }
            set
            {
                if (email == value) return;

                email = value;
                userEmail = EmailType.FromString(value);

                LoginCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged(nameof(Email));
                RaisePropertyChanged(nameof(EmailIsValid));
            }
        }

        public string Password { get; set; }

        public IMvxCommand LoginCommand { get; }

        public LoginViewModel(IApiFactory apiFactory)
        {
            this.apiFactory = apiFactory;
            LoginCommand = new MvxCommand(login, loginCanExecute);
        }

        public override Task Initialize(LoginParameter parameter)
            => Task.FromResult(0);
       
        public bool EmailIsValid => userEmail.IsValid;

        private void login()
        {
            loginDisposable =
                DataSource.User
                          .Login(userEmail, Password)
                          .Subscribe(onUser, onError);

            LoginCommand.RaiseCanExecuteChanged();
        }

        private bool loginCanExecute() 
            => loginDisposable == null && EmailIsValid;

        private void onUser(IUser user)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();

            var credentials = Credentials.WithApiToken(user.ApiToken);
            var api = apiFactory.CreateApiWith(credentials);
            var database = Mvx.Resolve<ITogglDatabase>();
            var dataSource = new TogglDataSource(database, api);

            Mvx.RegisterSingleton<ITogglApi>(api);
            Mvx.RegisterSingleton<ITogglDataSource>(dataSource);
        }

        private void onError(Exception ex)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}