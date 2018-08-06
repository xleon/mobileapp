using System;
using FluentAssertions;
using NUnit.Framework;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class CalendarViewModelTests
    {
        public abstract class CalendarViewModelTest : BaseViewModelTests<CalendarViewModel>
        {
            protected override CalendarViewModel CreateViewModel()
                => new CalendarViewModel(
                    TimeService,
                    InteractorFactory,
                    PermissionsService,
                    NavigationService);
        }

        public sealed class TheConstructor : CalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useTimeService,
                bool useInteractorFactory,
                bool useNavigationService,
                bool usePermissionsService)
            {
                var timeService = useTimeService ? TimeService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var permissionsService = usePermissionsService ? PermissionsService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarViewModel(
                        timeService,
                        interactorFactory,
                        permissionsService,
                        navigationService);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }
    }
}
