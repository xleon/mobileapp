using System.Reactive.Linq;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac.Models;
using System;
using PropertyChanged;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [ImplementPropertyChanged]
    public class LoginViewModel : BaseViewModel
    {
        private IDisposable loginDisposable;

        public string Email { get; set; }

        public string Password { get; set; }

        public IMvxCommand LoginCommand { get; }

        public LoginViewModel()
        {
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
        }

        private void onError(Exception ex)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}