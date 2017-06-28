using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using EmailType = Toggl.Multivac.Email;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class LoginViewModel : BaseViewModel<LoginParameter>
    {
        private readonly ILoginManager loginManager;

        private IDisposable loginDisposable;

        private EmailType userEmail = EmailType.Invalid;

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

        public LoginViewModel(ILoginManager loginManager)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));

            this.loginManager = loginManager;

            LoginCommand = new MvxCommand(login, loginCanExecute);
        }

        public override Task Initialize(LoginParameter parameter)
            => Task.FromResult(0);
       
        public bool EmailIsValid => userEmail.IsValid;

        private void login()
        {
            loginDisposable =
                loginManager
                    .Login(userEmail, Password)
                    .Subscribe(onDataSource, onError);

            LoginCommand.RaiseCanExecuteChanged();
        }

        private bool loginCanExecute() 
            => loginDisposable == null && EmailIsValid;

        private void onDataSource(ITogglDataSource dataSource)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();  

            Mvx.RegisterSingleton(dataSource);
        }

        private void onError(Exception ex)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}