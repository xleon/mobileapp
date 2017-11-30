using System;
using MvvmCross.Core.Navigation;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.MvvmCross.Services
{
    public sealed class ApiErrorHandlingService : IApiErrorHandlingService
    {
        private readonly IMvxNavigationService navigationService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        public ApiErrorHandlingService(
            IMvxNavigationService navigationService,
            IAccessRestrictionStorage accessRestrictionStorage)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            this.navigationService = navigationService;
            this.accessRestrictionStorage = accessRestrictionStorage;
        }

        public bool TryHandleDeprecationError(Exception error)
        {
            switch (error)
            {
                case ApiDeprecatedException _:
                    accessRestrictionStorage.SetApiOutdated();
                    navigationService.Navigate<OutdatedAppViewModel>();
                    return true;
                case ClientDeprecatedException _:
                    accessRestrictionStorage.SetClientOutdated();
                    navigationService.Navigate<OutdatedAppViewModel>();
                    return true;
            }

            return false;
        }

        public bool TryHandleUnauthorizedError(Exception error)
        {
            if (error is UnauthorizedException)
            {
                accessRestrictionStorage.SetUnauthorizedAccess();
                navigationService.Navigate<TokenResetViewModel>();
                return true;
            }

            return false;
        }
    }
}
