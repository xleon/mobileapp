using System;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Shared;

namespace Toggl.Core.UI
{
    public abstract class UIDependencyContainer : DependencyContainer
    {
        private readonly Lazy<IBrowserService> browserService;
        private readonly Lazy<IPermissionsChecker> permissionsService;
        private readonly Lazy<INavigationService> navigationService;
        private readonly Lazy<IUrlHandler> urlHandler;
        private readonly Lazy<IPasswordManagerService> passwordManagerService;

        public IBrowserService BrowserService => browserService.Value;
        public IPermissionsChecker PermissionsChecker => permissionsService.Value;
        public INavigationService NavigationService => navigationService.Value;

        public IUrlHandler UrlHandler => urlHandler.Value;

        public IPasswordManagerService PasswordManagerService => passwordManagerService.Value;

        public static UIDependencyContainer Instance { get; protected set; }

        protected UIDependencyContainer(ApiEnvironment apiEnvironment, UserAgent userAgent)
            : base(apiEnvironment, userAgent)
        {
            browserService = new Lazy<IBrowserService>(CreateBrowserService);
            permissionsService = new Lazy<IPermissionsChecker>(CreatePermissionsChecker);
            navigationService = new Lazy<INavigationService>(CreateNavigationService);
            urlHandler = new Lazy<IUrlHandler>(CreateUrlHandler);
            passwordManagerService = new Lazy<IPasswordManagerService>(CreatePasswordManagerService);
        }

        protected abstract IBrowserService CreateBrowserService();
        protected abstract IPermissionsChecker CreatePermissionsChecker();
        protected abstract INavigationService CreateNavigationService();
        protected abstract IUrlHandler CreateUrlHandler();
        protected abstract IPasswordManagerService CreatePasswordManagerService();

        protected override IErrorHandlingService CreateErrorHandlingService()
            => new ErrorHandlingService(NavigationService, AccessRestrictionStorage);
    }
}
