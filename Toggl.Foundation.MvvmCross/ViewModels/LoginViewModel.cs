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

        public LoginViewModel()
        {
            LoginCommand = new MvxCommand(login, loginCanExecute);
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public IMvxCommand LoginCommand { get; }

        private void login()
        {
            loginDisposable =
                DataSource.User
                          .Login(Email, Password)
                          .Subscribe(OnUser, OnError);

            LoginCommand.RaiseCanExecuteChanged();
        }

        private bool loginCanExecute() => loginDisposable == null;

        private void OnUser(IUser user)
        {
            
        }

        private void OnError(Exception ex)
        {
            loginDisposable = null;
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}