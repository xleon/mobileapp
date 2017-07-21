using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;
using Toggl.Ultrawave.Exceptions;
using EmailType = Toggl.Multivac.Email;
using LoginType = Toggl.Foundation.MvvmCross.Parameters.LoginParameter.LoginType;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class LoginViewModel : BaseViewModel<LoginParameter>
    {
        public const int EmailPage = 0;
        public const int PasswordPage = 1;

        private readonly ILoginManager loginManager;
        private readonly IMvxNavigationService navigationService;
        private readonly IPasswordManagerService passwordManagerService;

        private IDisposable loginDisposable;
        private IDisposable passwordManagerDisposable;

        private EmailType email = EmailType.Invalid;

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string ErrorText { get; set; } = "";

        [DependsOn(nameof(ErrorText))]
        public bool HasError => !string.IsNullOrEmpty(ErrorText);

        public int CurrentPage { get; private set; } = EmailPage;

        public bool IsLoading { get; private set; } = false;

        public bool IsPasswordMasked { get; private set; } = true;

        public LoginType LoginType { get; set; }

        public IMvxCommand NextCommand { get; }

        public IMvxCommand BackCommand { get; }

        public IMvxCommand TogglePasswordVisibilityCommand { get; }

        public IMvxCommand StartPasswordManagerCommand { get; }

        [DependsOn(nameof(CurrentPage))]
        public bool IsEmailPage => CurrentPage == EmailPage;

        [DependsOn(nameof(CurrentPage))]
        public bool IsPasswordPage => CurrentPage == PasswordPage;

        [DependsOn(nameof(CurrentPage), nameof(Password))]
        public bool NextIsEnabled
            => IsEmailPage ? email.IsValid : (Password.Length > 0 && !IsLoading);

        public bool IsPasswordManagerAvailable
            => passwordManagerService.IsAvailable;

        public LoginViewModel(ILoginManager loginManager, IMvxNavigationService navigationService, IPasswordManagerService passwordManagerService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(passwordManagerService, nameof(passwordManagerService));

            this.loginManager = loginManager;
            this.navigationService = navigationService;
            this.passwordManagerService = passwordManagerService;

            BackCommand = new MvxCommand(back);
            NextCommand = new MvxCommand(next);
            StartPasswordManagerCommand = new MvxCommand(startPasswordManager);
            TogglePasswordVisibilityCommand = new MvxCommand(togglePasswordVisibility);
        }

        public override Task Initialize(LoginParameter parameter)
        {
            LoginType = parameter.Type;
            Title = LoginType == LoginType.Login ? Resources.LoginTitle : Resources.SignUpTitle;

            return base.Initialize();
        }

        private void OnEmailChanged()
        {
            email = EmailType.FromString(Email);
            RaisePropertyChanged(nameof(NextIsEnabled));
        }

        private void next()
        {
            if (!NextIsEnabled) return;

            if (IsPasswordPage) login();

            CurrentPage = PasswordPage;
            ErrorText = "";
        }

        private void back()
        {
            if (IsEmailPage)
                navigationService.Close(this);

            CurrentPage = EmailPage;
            ErrorText = "";
        }

        private void togglePasswordVisibility()
            => IsPasswordMasked = !IsPasswordMasked;

        private void login()
        {
            IsLoading = true;

            loginDisposable =
                loginManager
                    .Login(email, Password)
                    .Subscribe(onDataSource, onError, onCompleted);
        }

        private void startPasswordManager()
        {
            if (!passwordManagerService.IsAvailable) return;
            if (passwordManagerDisposable != null) return;

            passwordManagerDisposable =
                passwordManagerService
                    .GetLoginInformation()
                    .Subscribe(onLoginInfo, onError, onCompleted);
        }

        private void onLoginInfo(PasswordManagerResult loginInfo)
        {
            Email = loginInfo.Email;
            if (!NextIsEnabled) return;
 
            CurrentPage = PasswordPage;
            Password = loginInfo.Password;
            if (!NextIsEnabled) return;

            login();
        }

        private void onDataSource(ITogglDataSource dataSource)
        {
            Mvx.RegisterSingleton(dataSource);

            navigationService.Navigate<MainViewModel>();
        }

        private void onError(Exception ex)
        {
            ErrorText = ex is NotAuthorizedException ? Resources.IncorrectEmailOrPassword
                                                     : Resources.GenericLoginError;

            onCompleted();
        }

        private void onCompleted()
        {
            IsLoading = false;

            loginDisposable?.Dispose();
            passwordManagerDisposable?.Dispose();

            loginDisposable = null;
            passwordManagerDisposable = null;
        }
    }
}
