using System;
using FluentAssertions;
using MvvmCross.Core.Navigation;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Services
{
    public sealed class ApiErrorHandlingServiceTests
    {
        public abstract class BaseApiErrorHandlingServiceTests
        {
            protected readonly IMvxNavigationService NavigationService;
            protected readonly IAccessRestrictionStorage AccessRestrictionStorage;
            protected readonly IApiErrorHandlingService ApiErrorHandlingService;

            public BaseApiErrorHandlingServiceTests()
            {
                NavigationService = Substitute.For<IMvxNavigationService>();
                AccessRestrictionStorage = Substitute.For<IAccessRestrictionStorage>();
                ApiErrorHandlingService =
                    new ApiErrorHandlingService(NavigationService, AccessRestrictionStorage);
            }
        }

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useNavigationService, bool useAccessRestrictionStorage)
            {
                var navigationService = useNavigationService ? Substitute.For<IMvxNavigationService>() : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? Substitute.For<IAccessRestrictionStorage>() : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ApiErrorHandlingService(navigationService, accessRestrictionStorage);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheApiDeprecatedException : BaseApiErrorHandlingServiceTests
        {
            private ApiDeprecatedException exception => new ApiDeprecatedException(Substitute.For<IRequest>(), Substitute.For<IResponse>());

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForApiDeprecatedException()
            {
                var result = ApiErrorHandlingService.TryHandleDeprecationError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForApiDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.Received().SetApiOutdated();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForApiDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.Received().Navigate<OutdatedAppViewModel>();
            }
        }

        public sealed class TheClientDeprecatedException : BaseApiErrorHandlingServiceTests
        {
            private ClientDeprecatedException exception => new ClientDeprecatedException(Substitute.For<IRequest>(), Substitute.For<IResponse>());

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForClientDeprecatedException()
            {
                var result = ApiErrorHandlingService.TryHandleDeprecationError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForClientDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.Received().SetClientOutdated();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForClientDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.Received().Navigate<OutdatedAppViewModel>();
            }
        }

        public sealed class TheOtherExceptionsThanDeprecationExceptions : BaseApiErrorHandlingServiceTests
        {
            private Exception exception => new Exception();

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.DidNotReceive().Navigate<OutdatedAppViewModel>();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRestrictAccessForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.DidNotReceive().SetApiOutdated();
                AccessRestrictionStorage.DidNotReceive().SetClientOutdated();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotNavigateForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.DidNotReceive().Navigate<OutdatedAppViewModel>();
            }
        }

        public sealed class TheUnauthorizedException : BaseApiErrorHandlingServiceTests
        {
            private UnauthorizedException exception => new UnauthorizedException(Substitute.For<IRequest>(), Substitute.For<IResponse>());

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForClientDeprecatedException()
            {
                var result = ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForClientDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                AccessRestrictionStorage.Received().SetUnauthorizedAccess();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForClientDeprecatedException()
            {
                ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.Received().Navigate<TokenResetViewModel>();
            }
        }

        public sealed class TheOtherExceptionsThanUnauthorizedException : BaseApiErrorHandlingServiceTests
        {
            private Exception exception => new Exception();

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRestrictAccessForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                AccessRestrictionStorage.DidNotReceive().SetUnauthorizedAccess();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotNavigateForDifferentExceptions()
            {
                ApiErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>();
            }
        }
    }
}
